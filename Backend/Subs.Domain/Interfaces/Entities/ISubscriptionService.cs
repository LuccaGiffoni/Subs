using Subs.Domain.Enums;
using Subs.Domain.Models;

namespace Subs.Domain.Interfaces.Entities;

/// <summary>
/// Defines methods for managing subscription operations.
/// </summary>
/// <remarks>This interface provides functionality to create and manage subscriptions. Implementations of this
/// interface should handle the specific details of subscription creation  and any associated operations.
/// </remarks>
public interface ISubscriptionService
{
    /// <summary>
    /// Creates a new subscription with the specified details and performs the specified operation.
    /// </summary>
    /// <remarks>The <paramref name="operation"/> parameter determines the specific action to be performed
    /// during the creation process. Ensure that the provided operation is supported by the system.</remarks>
    /// <param name="subscription">The subscription object containing the details of the subscription to create. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created <see
    /// cref="Subscription"/> object.</returns>
    public Task<Subscription> Create(Subscription subscription);

    /// <summary>
    /// Updates the specified subscription based on the provided operation and updated subscription details.
    /// </summary>
    /// <param name="subscription">The current subscription to be updated. Cannot be null.</param>
    /// <param name="updated">The updated subscription details to apply. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Update(Subscription subscription, Subscription updated);

    /// <summary>
    /// Updates the status of the specified subscription.
    /// </summary>
    /// <param name="subscription">The subscription to update. Cannot be <see langword="null"/>.</param>
    /// <param name="status">The new status to apply to the subscription.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created <see
    /// cref="Subscription"/></returns>
    public Task<Subscription> UpdateStatus(Subscription subscription, EStatus status);

    /// <summary>
    /// Deletes the specified subscription.
    /// </summary>
    /// <param name="subscription">The subscription to delete. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public Task Delete(Subscription subscription);
}