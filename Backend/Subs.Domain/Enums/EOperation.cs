using System.ComponentModel;

namespace Subs.Domain.Enums;

/// <summary>
/// Represents the type of operation to be performed.
/// </summary>
/// <remarks>This enumeration is typically used to specify the action to be taken in scenarios such as
/// data manipulation or resource management.
/// </remarks>
/// <value>
/// <see cref="Create"/>: Indicates that a new resource or record should be created.<br/>
/// <see cref="Update"/>: Indicates that an existing resource or record should be updated.<br/>
/// <see cref="Delete"/>: Indicates that an existing resource or record should be deleted.<br/>
/// <see cref="Rollback"/> : Indicates that a rollback event has occurred in the system.<br/>
/// </value>
public enum EOperation
{
    /// <summary>
    /// Creates a new instance of the specified object or resource.
    /// </summary>
    [Description("Create a new resource or record.")]
    Create,

    /// <summary>
    /// Updates the current state or data of the object.
    /// </summary>
    [Description("Update a resource or record.")]
    Update,

    /// <summary>
    /// Deletes the specified resource or entity.
    /// </summary>
    [Description("Delete a resource or record.")]
    Delete,

    /// <summary>
    /// Represents a rollback event in the system.
    /// </summary>
    [Description("Rollback event")]
    Rollback
}