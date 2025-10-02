using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.Domain.Interfaces.Entities;

/// <summary>
/// Defines operations for managing client entities.
/// </summary>
/// <remarks>
/// This interface provides methods to create, update, and delete client entities. 
/// Implementations of this interface should handle the persistence and validation of client data.
/// </remarks>
public interface IClientService
{
    /// <summary>
    /// Creates a new client in the system.
    /// </summary>
    /// <param name="client">The client object containing the details of the client to be created. Cannot be null.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the created <see cref="Client"/>
    /// object with updated details, such as its unique identifier.
    /// </returns>
    public Task<Client> Create(Client client);

    /// <summary>
    /// Updates the properties of an existing client with the values from the updated client.
    /// </summary>
    /// <remarks>This method updates the properties of the <paramref name="client"/> object to match
    /// those of the  <paramref name="updated"/> object. Only the exposed properties of the <paramref
    /// name="client"/>  will be modified. Ensure that both parameters are valid and non-null before calling this
    /// method.</remarks>
    /// <param name="client">The existing client to be updated. Must not be <see langword="null"/>.</param>
    /// <param name="updated">The client containing the updated values. Must not be <see langword="null"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated client.</returns>
    public Task<Client> Update(Client client, Client updated);

    /// <summary>
    /// Deletes the specified client from the system.
    /// </summary>
    /// <param name="client">The client to be deleted. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public Task Delete(Client client);
}