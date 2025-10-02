using System.ComponentModel;

namespace Subs.Domain.Enums;

/// <summary>
/// Defines the type of discount applied to a subscription or product
/// </summary>
/// <value>
/// <see cref="Absolute"/>: A fixed amount discount. <br/>
/// <see cref="Percentage"/>: A percentage-based discount. <br/>
/// </value>
public enum EDiscountType
{
    /// <summary>
    /// Decimal value representing a fixed amount discount
    /// </summary>
    [Description("Absolute discount")]
    Absolute,

    /// <summary>
    /// Percentage value representing a relative discount
    /// </summary>
    [Description("Relative discount")]
    Percentage
}