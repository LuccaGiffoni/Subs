using System.ComponentModel;

namespace Subs.Domain.Enums;

/// <summary>
/// Represents the status of a message during its lifecycle.
/// </summary>
/// <remarks>
/// This enumeration is used to indicate the current state of a message as it progresses through a system.
/// </remarks>
/// <value>
/// <see cref="Received"/>: The message has been received but not yet processed.<br/>
/// <see cref="Processing"/>: The message is currently being processed.<br/>
/// <see cref="Processed"/>: The message has been successfully processed.<br/>
/// <see cref="Failed"/>: The processing of the message has failed.
/// </value>
public enum EMessageStatus
{
    /// <summary>
    /// Represents the state of the message when it has been received.
    /// </summary>
    [Description("Message received")]
    Received,

    /// <summary>
    /// Represents the processing state or functionality within the application.
    /// </summary>
    [Description("Message is being processed")]
    Processing,

    /// <summary>
    /// Indicates that the message has been processed successfully.
    /// </summary>
    [Description("Message has been processed successfully")]
    Processed,

    /// <summary>
    /// Represents a state where message processing has failed.
    /// </summary>
    [Description("Message processing has failed")]
    Failed
}