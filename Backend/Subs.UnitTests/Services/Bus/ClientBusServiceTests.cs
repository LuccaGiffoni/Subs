using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Subs.Core.Data;
using Subs.Core.Services.Bus;
using Subs.Domain.Enums;
using Subs.Domain.Models.Messages;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.UnitTests.Services.Bus;

public class ClientBusServiceTests
{
    private SubsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SubsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SubsDbContext(options);
    }

    private ClientMessage CreateMessage()
    {
        return new ClientMessage(
            new Client
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test@user.com",
                Phone = "123456789"
            },
            EOperation.Create
        );
    }

    [Fact]
    public async Task Send_ShouldAddMessageToDatabase()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var loggerMock = new Mock<ILogger<ClientBusService>>();
        var service = new ClientBusService(dbContext, loggerMock.Object);
        var message = CreateMessage();

        // Act
        await service.Send(message);

        // Assert
        var saved = await dbContext.ClientMessages.FirstOrDefaultAsync(m => m.Id == message.Id);
        Assert.NotNull(saved);
        Assert.Equal(EMessageStatus.Received, saved.Status);
        Assert.True(saved.CallbackAt > saved.CreatedAt);
    }

    [Fact]
    public async Task Receive_ShouldReturnAndProcessMessage()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var loggerMock = new Mock<ILogger<ClientBusService>>();
        var service = new ClientBusService(dbContext, loggerMock.Object);
        var message = CreateMessage();
        message.Status = EMessageStatus.Received;
        message.CallbackAt = DateTime.UtcNow.AddSeconds(-1);
        dbContext.ClientMessages.Add(message);
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
        var loggerMock = new Mock<ILogger<ClientBusService>>();
        var service = new ClientBusService(dbContext, loggerMock.Object);

        // Act
        var result = await service.Receive(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}