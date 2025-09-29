using Subs.Domain.Enums;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.Domain.Models;

/// <summary>
/// Represents a subscription
/// </summary>
public class Subscription
{
    /// <summary>
    /// Unique identifier for the subscription.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Timestamp when the subscription was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the subscription was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The current status of the subscription.
    /// </summary>
    public EStatus Status { get; set; }

    /// <summary>
    /// Details about the payment method and frequency
    /// </summary>
    public required Payment Payment { get; set; }

    /// <summary>
    /// Basic information about the client
    /// </summary>
    public required Client Client { get; set; }

    /// <summary>
    /// Identifier of the product associated with this subscription
    /// </summary>
    public Guid ProductId { get; set; }
}