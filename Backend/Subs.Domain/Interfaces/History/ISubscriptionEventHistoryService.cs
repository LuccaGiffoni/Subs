using Subs.Domain.Enums;
using Subs.Domain.Models;

namespace Subs.Domain.Interfaces.History;

/// <summary>
/// Provides functionality for managing and recording the history of events associated with entities.
/// </summary>
/// <remarks>
/// This service is used to log and track events related to entity operations, such as
/// status changes or rollback actions. It allows asynchronous addition of events to a entity's history,
/// ensuring that entity-related activities are properly recorded for auditing or troubleshooting
/// purposes.
/// </remarks>
public interface ISubscriptionEventHistoryService
{
    /// <summary>
    /// Asynchronously adds a new event to the specified entity with the given status and optional details.
    /// </summary>
    /// <remarks>Use this method to log events related to a entity, such as status changes or
    /// rollback operations. Ensure that the <paramref name="subscription"/> is valid and that the entity
    /// exists before calling this method.</remarks>
    /// <param name="subscription">The unique identifier of the entity to which the event belongs.</param>
    /// <param name="operation">The type of the event to be added.</param>
    /// <param name="statusAtEvent">The status of the entity at the time of the event. This parameter is optional and can be null.</param>
    /// <param name="note">An optional note providing additional information about the event. Defaults to an empty string.</param>
    /// <param name="rollbackId">An optional identifier for a related rollback event.  If provided, it associates the new event with a
    /// rollback operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AddEvent(Subscription subscription, 
                         EOperation operation,
                         EStatus statusAtEvent,
                         string note = "", 
                         Guid? rollbackId = null);
}