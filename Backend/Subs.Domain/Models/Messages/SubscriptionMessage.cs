using Subs.Domain.Enums;

namespace Subs.Domain.Models.Messages;

/// <summary>
/// Represents a message entity that tracks the details of an operation performed on a subscription
/// </summary>
/// <remarks>
/// A message contains metadata about an operation, including its unique identifier, the associated
/// subscription, the type of operation performed, timestamps for creation and processing, and the current status of the
/// message.
/// </remarks>
public class SubscriptionMessage : Message
{
    public Guid SubscriptionId { get; set; } 

    public SubscriptionMessage(Guid subscriptionId, EOperation operation) : base(operation)
    {
        SubscriptionId = subscriptionId;
        Operation = operation;
    }
}
