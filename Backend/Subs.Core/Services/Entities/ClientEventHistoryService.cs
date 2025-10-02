using Subs.Core.Data;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.History;
using Subs.Domain.Models;
using Subs.Domain.Models.History;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.Core.Services.Entities;

public class ClientEventHistoryService(SubsDbContext db) : IClientEventHistoryService
{
    private readonly SubsDbContext _db = db;

    /// <summary>
    /// Adds an event to the system for the specified client, describing an operation and optional details.
    /// </summary>
    /// <param name="client">The client associated with the event. Cannot be <see langword="null"/>.</param>
    /// <param name="operation">The operation being recorded. Must be a valid <see cref="EOperation"/> value.</param>
    /// <param name="note">An optional note providing additional context for the event. Defaults to an empty string if not specified.</param>
    /// <param name="rollbackId">An optional identifier for rollback purposes. If provided, associates the event with a rollback operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task AddEvent(Client client,
                               EOperation operation,
                               EStatus status,
                               string note = "",
                               Guid? rollbackId = null)
    {
        var history = new ClientEventHistory
        {
            Id = Guid.NewGuid(),
            Client = client,
            ClientId = client.Id,
            Operation = operation,
            StatusAtEvent = status,
            Note = note,
            CreatedAt = DateTime.UtcNow,
            RollbackId = rollbackId ?? Guid.Empty
        };

        _db.ClientEventHistories.Add(history);
        await _db.SaveChangesAsync();
    }
}