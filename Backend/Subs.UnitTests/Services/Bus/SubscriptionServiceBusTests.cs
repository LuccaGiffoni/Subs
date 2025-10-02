using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Subs.Core.Data;
using Subs.Core.Services.Bus;
using Subs.Domain.Enums;
using Subs.Domain.Models.Messages;

namespace Subs.UnitTests.Services.Bus;

public class SubscriptionBusServiceTests
{
    private SubsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SubsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SubsDbContext(options);
    }

    private SubscriptionMessage CreateMessage()
    {
        var subscriptionId = Guid.NewGuid();
        var message = new SubscriptionMessage(subscriptionId, EOperation.Create)
        {
            Status = EMessageStatus.Received,
            CallbackAt = DateTime.UtcNow.AddSeconds(-1)
        };
        return message;
    }

    [Fact]
    public async Task Send_ShouldAddMessageToDatabase()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var loggerMock = new Mock<ILogger<SubscriptionBusService>>();
        var service = new SubscriptionBusService(dbContext, loggerMock.Object);
        var message = CreateMessage();

        // Act
        await service.Send(message);

        // Assert
        var saved = await dbContext.SubscriptionMessages.FirstOrDefaultAsync(m => m.Id == message.Id);
        Assert.NotNull(saved);
        Assert.Equal(EMessageStatus.Received, saved.Status);
    }

    [Fact]
    public async Task Receive_ShouldReturnAndProcessMessage()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var loggerMock = new Mock<ILogger<SubscriptionBusService>>();
        var service = new SubscriptionBusService(dbContext, loggerMock.Object);
        var message = CreateMessage();
        dbContext.SubscriptionMessages.Add(message);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.Receive(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(EMessageStatus.Processed, result.Status);
        Assert.NotNull(result.ProcessedAt);
    }

    [Fact]
    public async Task Receive_ShouldReturnNull_WhenNoMessageAvailable()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var loggerMock = new Mock<ILogger<SubscriptionBusService>>();
        var service = new SubscriptionBusService(dbContext, loggerMock.Object);

        // Act
        var result = await service.Receive(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}