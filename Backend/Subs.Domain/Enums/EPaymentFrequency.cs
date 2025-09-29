using System.ComponentModel;

namespace Subs.Domain.Enums;

/// <summary>
/// Defines the frequency at which payments are made for a subscription
/// </summary>
/// <value>
/// <see cref="Bullet"/>: Single lump sum payment. <br/>
/// <see cref="Monthly"/>: Payments are made every month. <br/>
/// <see cref="Bimonthly"/>: Payments are made every two months. <br/>
/// <see cref="Quarterly"/>: Payments are made every three months. <br/>
/// <see cref="Semiannual"/>: Payments are made every six months. <br/>
/// <see cref="Annual"/>: Payments are made once a year. <br/>
/// </value>
public enum EPaymentFrequency
{
    /// <summary>
    /// Payment is made in a single lump sum at the end or start of the subscription period
    /// </summary>
    [Description("Single lump sum payment")]
    Bullet = 0,

    /// <summary>
    /// Payments are made every month
    /// </summary>
    [Description("Every month payment")]
    Monthly = 1,

    /// <summary>
    /// Payments are made every two months
    /// </summary>
    [Description("Every two months payment")]
    Bimonthly = 2,

    /// <summary>
    /// Payments are made every three months
    /// </summary>
    [Description("Every three months payment")]
    Quarterly = 3,

    /// <summary>
    /// Payments are made every six months
    /// </summary>
    [Description("Every six months payment")]
    Semiannual = 6,

    /// <summary>
    /// Payments are made once a year
    /// </summary>
    [Description("Once a year payment")]
    Annual = 12
}