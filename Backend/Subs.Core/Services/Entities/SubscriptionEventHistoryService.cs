using Microsoft.Extensions.Logging;
using Subs.Core.Data;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.History;
using Subs.Domain.Models;
using Subs.Domain.Models.History;

namespace Subs.Core.Services.Entities;

/// <summary>
/// Provides functionality for managing and recording the history of events associated with subscriptions.
/// </summary>
/// <remarks>
/// This service is used to log and track events related to subscription operations, such as
/// status changes or rollback actions. It allows asynchronous addition of events to a subscription's history,
/// ensuring that subscription-related activities are properly recorded for auditing or troubleshooting
/// purposes.
/// </remarks>
public class SubscriptionEventHistoryService(SubsDbContext db, 
                                             ILogger<SubscriptionEventHistoryService> logger) : ISubscriptionEventHistoryService
{
    private readonly SubsDbContext _db = db;
    private readonly ILogger<SubscriptionEventHistoryService> _logger = logger;

    public async Task AddEvent(Subscription subscription, 
                               EOperation operation, 
                               EStatus status,
                               string note = "", 
                               Guid? rollbackId = null)
    {
        var history = new SubscriptionEventHistory
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription.Id,
            Operation = operation,
            StatusAtEvent = status,
            Note = note,
            CreatedAt = DateTime.UtcNow,
            RollbackId = rollbackId ?? Guid.Empty
        };

        _logger.LogInformation("Creating event history for subscription {id}", subscription.Id);
        _db.SubscriptionsEventHistories.Add(history);
    }
}