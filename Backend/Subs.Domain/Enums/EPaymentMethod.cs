using System.ComponentModel;

namespace Subs.Domain.Enums;

/// <summary>
/// Represents the various payment methods available in the subscription system
/// </summary>
/// /// <value>
/// <see cref="Credit"/>: Credit card payment.. <br/>
/// <see cref="Debit"/>: Debit card payment. <br/>
/// <see cref="PIX"/>: Instantaneous brazilian's payment system. <br/>
/// <see cref="BankSlip"/>: Traditional bank slip payment. <br/>
/// </value>
public enum EPaymentMethod
{
    /// <summary>
    /// Credit card payment method
    /// </summary>
    [Description("Credit Card")]
    Credit,

    /// <summary>
    /// Debit card payment method
    /// </summary>
    [Description("Debit Card")]
    Debit,

    /// <summary>
    /// PIX payment method
    /// </summary>
    [Description("Instantaneous brazilian's payment system")]
    PIX,

    /// <summary>
    /// Bank slip payment method
    /// </summary>
    [Description("Traditional bank slip payment method")]
    BankSlip
}