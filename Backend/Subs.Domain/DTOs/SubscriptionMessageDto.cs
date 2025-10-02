namespace Subs.Domain.DTOs;

public class SubscriptionMessageDto
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    public string? SubscriptionJson { get; set; }
    public string Operation { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CallbackAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}