using Subs.Domain.Enums;

namespace Subs.Domain.Models.SubscriptionComponents;

/// <summary>
/// Represents a client in the subscription system.
/// </summary>
public class Client
{
    /// <summary>
    /// Unique client identification
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Client's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Client's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the client.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the client.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the operation.
    /// </summary>
    public EStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the collection of subscriptions associated with the current entity.
    /// </summary>
    public ICollection<Subscription> Subscriptions { get; set; } = [];
}