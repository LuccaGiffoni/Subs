using System.ComponentModel;

namespace Subs.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a subscription in the system.
/// </summary>
/// <value>
/// <see cref="Draft"/>: Initial creation, editable before submission. <br/>
/// <see cref="Pending"/>: Submitted and waiting for activation. <br/>
/// <see cref="Active"/>: Fully active and accessible to the customer. <br/>
/// <see cref="Canceled"/>: Terminated by customer request. <br/>
/// <see cref="Expired"/>: Automatically ended after its term. <br/>
/// <see cref="Suspended"/>: Temporarily disabled by the business, may be reactivated later. <br/>
/// </value>
/// <remarks>
/// Only one status is valid at a time for a subscription.<br/>
/// Transitions between statuses are controlled by business rules:
/// e.g., Draft → Pending → Active → (Suspended/Canceled/Expired).
/// </remarks>
public enum EStatus
{
    /// <summary>
    /// The subscription has been created but not yet finalized. 
    /// It is in draft mode and can be edited before submission.
    /// </summary>
    [Description("Subscription created but still on draft")]
    Draft,

    /// <summary>
    /// The subscription has been submitted and is waiting 
    /// to be processed or activated in the system.
    /// </summary>
    [Description("Subscription sent to queue to be created and activated")]
    Pending,

    /// <summary>
    /// The subscription is active and currently valid. 
    /// The customer has full access to the associated services.
    /// </summary>
    [Description("Subscription active")]
    Active,

    /// <summary>
    /// The subscription was canceled by the customer 
    /// and is no longer active.
    /// </summary>
    [Description("Subscription canceled by customer")]
    Canceled,

    /// <summary>
    /// The subscription has expired automatically, 
    /// typically because its term has ended.
    /// </summary>
    [Description("Subscription automatically expired")]
    Expired,

    /// <summary>
    /// The subscription has been temporarily suspended by the business. 
    /// The customer may not have access until it is reactivated.
    /// </summary>
    [Description("Subscription suspended by business")]
    Suspended
}
