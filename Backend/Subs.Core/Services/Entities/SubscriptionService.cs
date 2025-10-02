using Microsoft.Extensions.Logging;
using Subs.Core.Data;
using Subs.Core.Validators;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.Bus;
using Subs.Domain.Interfaces.Entities;
using Subs.Domain.Interfaces.History;
using Subs.Domain.Models;
using Subs.Domain.Models.Messages;
using Subs.Utils.Extensions;

namespace Subs.Core.Services.Entities;

/// <summary>
/// Defines methods for managing subscription operations.
/// </summary>
/// <remarks>
/// This interface provides functionality to create and manage subscriptions. Implementations of this
/// interface should handle the specific details of subscription creation  and any associated operations.
/// </remarks>
public class SubscriptionService(SubsDbContext db,
                                 IBusService<SubscriptionMessage> busService, 
                                 ISubscriptionEventHistoryService eventHistoryService,
                                 ILogger<SubscriptionService> logger) : ISubscriptionService
{
    private readonly SubsDbContext _db = db;
    private readonly IBusService<SubscriptionMessage> _bus = busService;
    private readonly ISubscriptionEventHistoryService _eventHistoryService = eventHistoryService;
    private readonly ILogger<SubscriptionService> _logger = logger;

    #region | Create
    public async Task<Subscription> Create(Subscription subscription)
    {
        try
        {
            subscription.Id = Guid.NewGuid();

            SetDates(subscription);
            Validate(subscription);

            if (subscription.Payment.Discount is null)
            {
                subscription.Payment.Discount = new()
                {
                    Type = EDiscountType.Percentage,
                    Value = 0
                };
            }

            var client = await _db.Clients.FindAsync(subscription.Client!.Id)
                ?? throw new ArgumentException("Client must exist and be active.");

            if (client.Status != EStatus.Active)
                throw new ArgumentException("Client must be active to create a subscription.");

            _db.Subscriptions.Add(subscription);
            await _eventHistoryService.AddEvent(subscription, EOperation.Create, EStatus.Draft, "Subscription created as draft.");
            
            var message = new SubscriptionMessage(subscription.Id, EOperation.Create);
            await _bus.Send(message);

            await _eventHistoryService.AddEvent(subscription, EOperation.Create, EStatus.Pending, "Subscription created and sent to processing queue.");
            await _db.SaveChangesAsync();
            _logger.LogInformation("Subscription {id} sent to creation queue.", subscription.Id);

            return subscription;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while creating subscription {subscriptionId}: {ex}", subscription.Id, e.Message);
            throw;
        }
    }

    private void Validate(Subscription subscription)
    {
        var validator = new SubscriptionValidator();
        var validationResult = validator.Validate(subscription);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogError("Invalid subscription: {errors}", errors);
            throw new ArgumentException(string.Join("; ", errors));
        }
    }

    private static void SetDates(Subscription subscription)
    {
        subscription.CreatedAt = subscription.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : subscription.CreatedAt.EnsureUtc();
        subscription.UpdatedAt = subscription.UpdatedAt == DateTime.MinValue ? DateTime.UtcNow : subscription.UpdatedAt.EnsureUtc();
        if (subscription.Payment?.Currency != null)
            subscription.Payment.Currency.Reference = subscription.Payment.Currency.Reference == DateTime.MinValue ? DateTime.UtcNow : subscription.Payment.Currency.Reference.EnsureUtc();
    }
    #endregion

    #region | Update
    public async Task Update(Subscription subscription, Subscription updated)
    {
        subscription.Status = updated.Status;
        subscription.UpdatedAt = DateTime.UtcNow;
        subscription.Payment = updated.Payment;
        subscription.Client = updated.Client;
        subscription.ProductId = updated.ProductId;

        Validate(subscription);

        var message = new SubscriptionMessage(subscription.Id, EOperation.Update);
        await _bus.Send(message);

        await _eventHistoryService.AddEvent(subscription, EOperation.Update, subscription.Status, "Subscription updated and sent to processing queue.");
        _logger.LogInformation("Subscription {id} sent to update queue.", subscription.Id);
        return;
    }

    public async Task<Subscription> UpdateStatus(Subscription subscription, EStatus status)
    {
        subscription.Status = status;
        subscription.UpdatedAt = DateTime.UtcNow;

        Validate(subscription);

        var message = new SubscriptionMessage(subscription.Id, EOperation.Update);
        await _bus.Send(message);

        await _eventHistoryService.AddEvent(subscription, EOperation.Update, subscription.Status, $"Subscription status updated to {status} and sent to processing queue.");
        _logger.LogInformation("Subscription {id} sent to update queue.", subscription.Id);
        return subscription;
    }
    #endregion

    #region | Delete
    public async Task Delete(Subscription subscription)
    {
        var message = new SubscriptionMessage(subscription.Id, EOperation.Delete);
        await _bus.Send(message);

        await _eventHistoryService.AddEvent(subscription, EOperation.Delete, subscription.Status, "Subscription sent to deletion queue.");
        _logger.LogInformation("Subscription {id} sent to deletion queue.", subscription.Id);
        return;
    }
    #endregion
}