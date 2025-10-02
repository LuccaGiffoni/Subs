namespace Subs.Domain.DTOs;

public class PaymentDto
{
    public required string Method { get; set; } = "Credit";
    public required string Frequency { get; set; } = "Bullet";
    public required decimal Amount { get; set; } = 0m;
    public required CurrencyDto Currency { get; set; }
    public DiscountDto? Discount { get; set; }
}
