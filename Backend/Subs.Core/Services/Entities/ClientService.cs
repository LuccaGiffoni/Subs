using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Subs.Core.Data;
using Subs.Core.Validators.SubscriptionComponents;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.Bus;
using Subs.Domain.Interfaces.Entities;
using Subs.Domain.Interfaces.History;
using Subs.Domain.Models.Messages;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.Core.Services.Entities;

/// <summary>
/// Provides operations for managing client entities, including creating, updating, and deleting clients.
/// </summary>
/// <remarks>
/// This service defines methods for handling client-related operations. Each method is
/// asynchronous and returns a <see cref="Task"/> to support non-blocking execution. Implementations of this
/// service are expected to handle persistence and validation of client data.
/// </remarks>
public class ClientService(SubsDbContext db,
                           IBusService<ClientMessage> busService,
                           IClientEventHistoryService eventHistoryService,
                           ILogger<ClientService> logger) : IClientService
{
    private readonly SubsDbContext _db = db;
    private readonly IBusService<ClientMessage> _bus = busService;
    private readonly IClientEventHistoryService _eventHistoryService = eventHistoryService;
    private readonly ILogger<ClientService> _logger = logger;

    public async Task<Client> Create(Client client)
    {
        if (client.Id == Guid.Empty)
            client.Id = Guid.NewGuid();

        Validate(client);

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        await _eventHistoryService.AddEvent(client, EOperation.Create, EStatus.Draft, "Client created as draft.");

        var message = new ClientMessage(client, EOperation.Create);
        await _bus.Send(message);

        await _eventHistoryService.AddEvent(client, EOperation.Create, EStatus.Pending, "Client sent to creation queue.");
        _logger.LogInformation("Client {id} sent to creation queue.", client.Id);
        return client;
    }

    public async Task<Client> Update(Client client, Client updated)
    {
        client.FirstName = updated.FirstName;
        client.LastName = updated.LastName;
        client.Email = updated.Email;
        client.Phone = updated.Phone;

        Validate(client);

        var message = new ClientMessage(client, EOperation.Update);
        await _bus.Send(message);

        await _eventHistoryService.AddEvent(client, EOperation.Update, client.Status, "Client updated and sent to processing queue.");
        _logger.LogInformation("Client {id} sent to update queue.", client.Id);
        return client;
    }

    public async Task Delete(Client client)
    {
        var message = new ClientMessage(client, EOperation.Delete);
        await _bus.Send(message);

        await _eventHistoryService.AddEvent(client, EOperation.Delete, client.Status, "Client deletion sent to processing queue.");
        _logger.LogInformation("Client {id} sent to delete queue.", client.Id);
    }

    private static void Validate(Client client)
    {
        var validator = new ClientValidator();
        var validationResult = validator.Validate(client);
        if (!validationResult.IsValid)
            throw new ArgumentException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
    }
}