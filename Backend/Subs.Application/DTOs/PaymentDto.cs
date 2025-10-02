namespace Subs.Application.Data;

public class PaymentDto
{
    public string Method { get; set; } = null!;
    public string Frequency { get; set; } = null!;
    public decimal Amount { get; set; }
    public DiscountDto? Discount { get; set; }
    public CurrencyDto Currency { get; set; } = null!;
}
