namespace Subs.Domain.DTOs;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status { get; set; } = null!;
    public Guid ClientId { get; set; }
    public ClientDto? Client { get; set; }
    public Guid ProductId { get; set; }
    public required PaymentDto Payment { get; set; }
}