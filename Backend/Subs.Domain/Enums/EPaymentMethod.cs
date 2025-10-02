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
    [Description("Credit")]
    Credit,

    /// <summary>
    /// Debit card payment method
    /// </summary>
    [Description("Debit")]
    Debit,

    /// <summary>
    /// PIX payment method
    /// </summary>
    [Description("PIX")]
    PIX,

    /// <summary>
    /// Bank slip payment method
    /// </summary>
    [Description("BankSlip")]
    BankSlip
}