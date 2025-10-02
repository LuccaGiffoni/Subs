using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Subs.Core.Data;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.Bus;
using Subs.Domain.Models.Messages;

namespace Subs.Core.Services.Bus;

public class SubscriptionBusService(SubsDbContext dbContext, ILogger<SubscriptionBusService> logger) : IBusService<SubscriptionMessage>
{
    private readonly SubsDbContext _dbContext = dbContext;
    private readonly ILogger<SubscriptionBusService> _logger = logger;

    public async Task Send(SubscriptionMessage message)
    {
        _dbContext.SubscriptionMessages.Add(message);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Subscription message ({messageId}) sent for Subscription {subscriptionId}", message.Id, message.SubscriptionId);
    }

    public async Task<SubscriptionMessage?> Receive(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var message = await _dbContext.SubscriptionMessages
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