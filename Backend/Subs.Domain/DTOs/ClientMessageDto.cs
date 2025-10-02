namespace Subs.Domain.DTOs;

public class ClientMessageDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string? ClientJson { get; set; }
    public string Operation { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CallbackAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}