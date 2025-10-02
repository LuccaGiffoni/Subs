using Subs.Domain.Models.Messages;

namespace Subs.Domain.Interfaces.Bus;

/// <summary>
/// Defines methods for interacting with an Azure Service Bus, including sending messages to a subscription and
/// receiving messages from the bus.
/// </summary>
/// <remarks>This interface provides an abstraction for working with Azure Service Bus, enabling message-based
/// communication between distributed systems. Implementations of this interface should handle the underlying
/// communication with Azure Service Bus.
/// </remarks>
public interface IBusService<T> where T : Message
{
    /// <summary>
    /// Sends a message to the specified message with the given operation.
    /// </summary>
    /// <param name="message">
    /// The message to which the message will be sent. Cannot be null.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous send operation.
    /// </returns>
    Task Send(T message);

    /// <summary>
    /// Asynchronously receives the next available message from the entity.
    /// </summary>
    /// <remarks>
    /// This method waits for a message to become available in the entity. If no message is
    /// available, it may return <see langword="null"/> depending on the implementation.
    /// </remarks>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. If the operation is canceled, the task will be completed with a
    /// <see cref="TaskCanceledException"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the next <see
    Task<T?> Receive(CancellationToken cancellationToken);
}