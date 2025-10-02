using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Subs.Core.Data;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.History;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.Core.Services.Worker;

public class ClientWorkerService(SubsDbContext db,
                                 IClientEventHistoryService clientEventHistoryService,
                                 ILogger<ClientWorkerService> logger)
{
    private readonly SubsDbContext _db = db;
    private readonly IClientEventHistoryService _clientEventHistoryService = clientEventHistoryService;
    private readonly ILogger<ClientWorkerService> _logger = logger;

    public async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var messages = await _db.ClientMessages
            .Where(m => m.Status == EMessageStatus.Received && m.CallbackAt <= now)
            .OrderBy(m => m.CallbackAt)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            switch (message.Operation)
            {
                case EOperation.Create:
                    var createdClient = await _db.Clients.FindAsync([message.Client!.Id], cancellationToken);
                    if (createdClient is null)
                    {
                        _logger.LogError("Client {id} not found in database.", message.Client.Id);
                        message.Status = EMessageStatus.Failed;
                        message.ProcessedAt = DateTime.UtcNow;

                        await _db.SaveChangesAsync(cancellationToken);
                        continue;
                    }

                    if (createdClient.Status == EStatus.Draft ||
                        createdClient.Status == EStatus.Pending)
                    {
                        createdClient.Status = EStatus.Active;
                        await _clientEventHistoryService.AddEvent(createdClient, EOperation.Create, createdClient.Status, "Client activated successfully.");
                    }
                    else
                    {
                        message.Status = EMessageStatus.Failed;
                        message.ProcessedAt = DateTime.UtcNow;
                        await _db.SaveChangesAsync(cancellationToken);

                        _logger.LogWarning("Client {id} is not in a valid state for creation.", message.Client.Id);
                        continue;
                    }
                    break;
                case EOperation.Update:
                    var client = await _db.Clients.FindAsync([message.Client!.Id], cancellationToken);
                    if (client != null)
                    {
                        client.FirstName = message.Client.FirstName;
                        client.LastName = message.Client.LastName;
                        client.Email = message.Client.Email;
                        client.Phone = message.Client.Phone;

                        await _clientEventHistoryService.AddEvent(client, EOperation.Update, client.Status, "Client updated successfully.");
                    }
                    break;
                case EOperation.Delete:
                    var clientId = message.Client!.Id;

                    var hasSubscriptions = await _db.Subscriptions.AnyAsync(s => s.ClientId == clientId, cancellationToken);
                    if (hasSubscriptions)
                    {
                        message.Status = EMessageStatus.Failed;
                        message.ProcessedAt = DateTime.UtcNow;
                        await _db.SaveChangesAsync(cancellationToken);

                        _logger.LogWarning("Client {id} cannot be deleted because they have associated subscriptions.", clientId);
                        continue;
                    }

                    var existingClient = await _db.Clients.FindAsync(clientId, cancellationToken);
                    if (existingClient != null)
                    {
                        _db.Clients.Remove(existingClient);
                        await _db.SaveChangesAsync(cancellationToken);
                    }

                    var associatedMessages = _db.ClientMessages.Where(m => m.ClientId == clientId && m.Status != EMessageStatus.Processed);
                    if (associatedMessages.Any())
                    {
                        foreach (var msg in associatedMessages)
                        {
                            msg.Status = EMessageStatus.Failed;
                            msg.ProcessedAt = DateTime.UtcNow;
                        }

                        await _db.SaveChangesAsync(cancellationToken);
                    }

                    var histories = _db.ClientEventHistories.Where(h => h.ClientId == clientId);
                    if (histories.Any())
                    {
                        _db.ClientEventHistories.RemoveRange(histories);
                        await _db.SaveChangesAsync(cancellationToken);
                    }
                    break;
            }

            message.Status = EMessageStatus.Processed;
            message.ProcessedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Client {id} message ({messageId}) processed!", message.Client!.Id, message.Id);
        }
    }
}