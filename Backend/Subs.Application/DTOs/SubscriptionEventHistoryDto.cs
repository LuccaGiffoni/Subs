namespace Subs.Application.Data;

public class SubscriptionEventHistoryDto
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid RollbackId { get; set; }
}
