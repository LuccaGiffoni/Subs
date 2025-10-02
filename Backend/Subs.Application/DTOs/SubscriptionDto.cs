namespace Subs.Application.Data;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status { get; set; } = null!;
    public Guid ClientId { get; set; }
    public Guid ProductId { get; set; }
    public PaymentDto Payment { get; set; } = null!;
}