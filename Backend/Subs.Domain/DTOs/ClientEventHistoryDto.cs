namespace Subs.Domain.DTOs;

public class ClientEventHistoryDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid RollbackId { get; set; }
}
