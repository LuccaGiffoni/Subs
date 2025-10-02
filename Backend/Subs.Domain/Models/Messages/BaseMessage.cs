using Subs.Domain.Enums;

namespace Subs.Domain.Models.Messages;

/// <summary>
/// Represents a message entity that tracks the details of an operation performed on an enity
/// </summary>
public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public EOperation Operation { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public EMessageStatus Status { get; set; } = EMessageStatus.Received;
    public DateTime CallbackAt { get; set; } = DateTime.UtcNow.AddSeconds(Random.Shared.Next(4, 7));
    public DateTime? ProcessedAt { get; set; }

    public Message()
    {
        
    }

    public Message(EOperation operation)
    {
        Operation = operation;
    }
}