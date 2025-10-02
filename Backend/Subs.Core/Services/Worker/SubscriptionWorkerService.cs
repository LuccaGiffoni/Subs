using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Subs.Core.Data;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.History;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.Core.Services.Worker;

public class SubscriptionWorkerService(SubsDbContext db,
                                       ISubscriptionEventHistoryService subscriptionEventHistoryService,
                                       ILogger<SubscriptionWorkerService> logger)
{
    private readonly SubsDbContext _db = db;
    private readonly ISubscriptionEventHistoryService _subscriptionEventHistoryService = subscriptionEventHistoryService;
    private readonly ILogger<SubscriptionWorkerService> _logger = logger;

    public async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var messages = await _db.SubscriptionMessages
            .Where(m => m.Status == EMessageStatus.Received && m.CallbackAt <= now)
            .OrderBy(m => m.CallbackAt)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            var subscription = await _db.Subscriptions.FindAsync([message.SubscriptionId], cancellationToken);

            if (subscription == null)
            {
                message.Status = EMessageStatus.Failed;
                message.ProcessedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogError("Subscription {subscriptionId} not found for message {messageId}", message.SubscriptionId, message.Id);
                continue;
            }

            var clientExists = await _db.Clients.AnyAsync(c => c.Id == subscription.ClientId, cancellationToken);
            if (!clientExists)
            {
                message.Status = EMessageStatus.Failed;
                message.ProcessedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogError("Client {clientId} for subscription {subscriptionId} does not exist.", subscription.ClientId, subscription.Id);
                continue;
            }

            switch (message.Operation)
            {
                case EOperation.Create:
                    subscription.Status = EStatus.Active;
                    _logger.LogInformation("Subscription {id} created and activated.", subscription.Id);
                    break;

                case EOperation.Update:
                    var existingSub = await _db.Subscriptions.FindAsync(subscription.Id);
                    if (existingSub != null)
                    {
                        existingSub.Status = subscription.Status;
                        existingSub.UpdatedAt = DateTime.UtcNow;
                        existingSub.Payment = subscription.Payment;
                        existingSub.ProductId = subscription.ProductId;
                    }
                    break;

                case EOperation.Delete:
                    var toDelete = await _db.Subscriptions.FindAsync(subscription.Id);
                    if (toDelete != null)
                    {
                        _db.Subscriptions.Remove(toDelete);
                    }
                    break;
            }

            message.Status = EMessageStatus.Processed;
            message.ProcessedAt = DateTime.UtcNow;

            await _subscriptionEventHistoryService.AddEvent(subscription, message.Operation, subscription.Status, $"Subscription {message.Operation} operation processed and is active.");
            await _db.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Subscription {id} message ({messageId}) processed.", subscription.Id, message.Id);
        }
    }
}