namespace Subs.Application.Data;

public class CurrencyDto
{
    public string Type { get; set; } = null!;
    public decimal Rate { get; set; }
    public string? Reference { get; set; }
}