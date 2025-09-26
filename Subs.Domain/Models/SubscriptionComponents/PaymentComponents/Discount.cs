using Subs.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subs.Domain.Models.SubscriptionComponents.PaymentComponents;

/// <summary>
/// Discount details for a payment
/// </summary>
public class Discount
{
    /// <summary>
    /// Generic amount for the discount (100% = 100)
    /// </summary>
    public decimal Value { get; set; } = 0.0m;

    /// <summary>
    /// Type that defines how the discount value should be interpreted
    /// </summary>
    public EDiscountType Type { get; set; } = EDiscountType.Absolute;

    /// <summary>
    /// Gets the percentage value of the discount if the type is Percentage; otherwise, returns 0.0
    /// </summary>
    [NotMapped]
    public decimal PercentageValue
        => Type == EDiscountType.Percentage ? Value / 100m : 0.0m;

    /// <summary>
    /// Calculates the discount amount based on the base amount and the discount type
    /// </summary>
    /// <param name="baseAmount"></param>
    /// <returns></returns>
    public decimal GetDiscountAmount(decimal baseAmount) 
        => Type == EDiscountType.Absolute ? Value : baseAmount * PercentageValue;
}