using Subs.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subs.Domain.Models;

/// <summary>
/// Represents the history of events for a subscription
/// </summary>
public class SubscriptionEventHistory
{
    /// <summary>
    /// Unique subscription history identification
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Subscription unique identification
    /// </summary>
    public Guid SubscriptionId { get; set; }

    /// <summary>
    /// Identifier of the rollback event, if applicable
    /// </summary>
    public Guid RollbackId { get; set; } = Guid.Empty;

    /// <summary>
    /// Timestamp when the history record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Subscription status set with this history record
    /// </summary>
    public EStatus Status { get; set; }

    /// <summary>
    /// Additional notes or comments about the event
    /// </summary>
    public string Note { get; set; } = string.Empty;

    #region | Get-only properties
    /// <summary>
    /// Indicates if this event was a rollback to a previous status
    /// </summary>
    [NotMapped]
    public bool RepresentsRollbackEvent { get => RollbackId != Guid.Empty; }
    #endregion
}