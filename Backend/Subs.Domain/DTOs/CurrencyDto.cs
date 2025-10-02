namespace Subs.Domain.DTOs;

public class CurrencyDto
{
    public required string Type { get; set; } = "Real";
    public required decimal Rate { get; set; } = 1.0m;
    public string? Reference { get; set; }
}