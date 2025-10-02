using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Subs.Core.Data;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.Bus;
using Subs.Domain.Models.Messages;

namespace Subs.Core.Services.Bus;

public class ClientBusService(SubsDbContext dbContext, ILogger<ClientBusService> logger) : IBusService<ClientMessage>
{
    private readonly SubsDbContext _dbContext = dbContext;
    private readonly ILogger<ClientBusService> _logger = logger;
    public async Task Send(ClientMessage message)
    {
        var delaySeconds = Random.Shared.Next(4, 7);
        message.CreatedAt = DateTime.UtcNow;
        message.Status = EMessageStatus.Received;
        message.CallbackAt = DateTime.UtcNow.AddSeconds(delaySeconds);

        _dbContext.ClientMessages.Add(message);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Client {id} message ({messageId}) sent!", message.Client.Id, message.Id);
    }

    public async Task<ClientMessage?> Receive(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var message = await _dbContext.ClientMessages
            .Where(m => m.Status == EMessageStatus.Received && m.CallbackAt <= now)
            .OrderBy(m => m.CallbackAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (message != null)
        {
            message.Status = EMessageStatus.Processed;
            message.ProcessedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return message;
        }

        return null;
    }
}