using Subs.Domain.Enums;
using Subs.Domain.Models.SubscriptionComponents.PaymentComponents;

namespace Subs.Domain.Models.SubscriptionComponents;

/// <summary>
/// Details about the subscription's payment
/// </summary>
public class Payment
{
    /// <summary>
    /// Subscription payment method (e.g., Credit Card, PayPal, Bank Transfer)
    /// </summary>
    public EPaymentMethod Method { get; set; }

    /// <summary>
    /// Payment frequency (e.g., Monthly, Yearly)
    /// </summary>
    public EPaymentFrequency Frequency { get; set; }

    /// <summary>
    /// Payment amount before any discounts
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Properties related to any discounts applied to the payment
    /// </summary>
    public Discount Discount { get; set; } = new();

    /// <summary>
    /// Currency details for the payment
    /// </summary>
    public Currency Currency { get; set; } = new();

    #region | Get-only properties
    /// <summary>
    /// Total amount after applying any type of discount
    /// </summary>
    public decimal TotalAmount { get => Amount - Discount.GetDiscountFor(Amount); }
    #endregion
}