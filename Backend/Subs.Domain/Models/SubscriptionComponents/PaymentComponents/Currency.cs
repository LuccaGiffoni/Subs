namespace Subs.Domain.Models.SubscriptionComponents.PaymentComponents;

/// <summary>
/// Properties related to the currency used for the payment
/// </summary>
public class Currency
{
    /// <summary>
    /// Payment currency type (e.g., Real, Dollar, Euro)
    /// </summary>
    public string Type { get; set; } = "Real";

    /// <summary>
    /// Exchange rate relative to the local currency, Real
    /// </summary>
    public decimal Rate { get; set; } = 1.0m;

    /// <summary>
    /// Reference date for the exchange rate
    /// </summary>
    public DateTime Reference { get; set; } = DateTime.Now;
}