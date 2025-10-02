using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Subs.Core.Data;
using Subs.Core.Services.Worker;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.History;
using Subs.Domain.Models;
using Subs.Domain.Models.Messages;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.UnitTests.Services.Worker;

public class ClientWorkerServiceTests
{
    private SubsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SubsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SubsDbContext(options);
    }

    private Client CreateClient(EStatus status = EStatus.Draft)
    {
        return new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@user.com",
            Phone = "5512991232566",
            Status = status
        };
    }

    private ClientMessage CreateMessage(Client client, EOperation operation)
    {
        return new ClientMessage(client, operation)
        {
            Status = EMessageStatus.Received,
            CallbackAt = DateTime.UtcNow.AddSeconds(-1)
        };
    }

    [Fact]
    public async Task ConsumeAsync_ShouldActivateClient_OnCreateOperation()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var historyMock = new Mock<IClientEventHistoryService>();
        var loggerMock = new Mock<ILogger<ClientWorkerService>>();
        var service = new ClientWorkerService(dbContext, historyMock.Object, loggerMock.Object);

        var client = CreateClient(EStatus.Draft);
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        var message = CreateMessage(client, EOperation.Create);
        dbContext.ClientMessages.Add(message);
        await dbContext.SaveChangesAsync();

        // Act
        await service.ConsumeAsync(CancellationToken.None);

        // Assert
        var updatedClient = await dbContext.Clients.FindAsync(client.Id);
        Assert.Equal(EStatus.Active, updatedClient.Status);
        historyMock.Verify(h => h.AddEvent(updatedClient, EOperation.Create, EStatus.Active, It.IsAny<string>(), null), Times.Once);
        var processedMessage = await dbContext.ClientMessages.FindAsync(message.Id);
        Assert.Equal(EMessageStatus.Processed, processedMessage.Status);
    }

    [Fact]
    public async Task ConsumeAsync_ShouldUpdateClient_OnUpdateOperation()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var historyMock = new Mock<IClientEventHistoryService>();
        var loggerMock = new Mock<ILogger<ClientWorkerService>>();
        var service = new ClientWorkerService(dbContext, historyMock.Object, loggerMock.Object);

        var client = CreateClient(EStatus.Active);
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        var updatedClient = new Client
        {
            Id = client.Id,
            FirstName = "Updated",
            LastName = "User",
            Email = "updated@user.com",
            Phone = "5512999999999",
            Status = EStatus.Active
        };
        var message = CreateMessage(updatedClient, EOperation.Update);
        dbContext.ClientMessages.Add(message);
        await dbContext.SaveChangesAsync();

        // Act
        await service.ConsumeAsync(CancellationToken.None);

        // Assert
        var dbClient = await dbContext.Clients.FindAsync(client.Id);
        Assert.Equal("Updated", dbClient.FirstName);
        Assert.Equal("updated@user.com", dbClient.Email);
        historyMock.Verify(h => h.AddEvent(dbClient, EOperation.Update, dbClient.Status, It.IsAny<string>(), null), Times.Once);
        var processedMessage = await dbContext.ClientMessages.FindAsync(message.Id);
        Assert.Equal(EMessageStatus.Processed, processedMessage.Status);
    }

    [Fact]
    public async Task ConsumeAsync_ShouldDeleteClient_WhenNoSubscriptions()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var historyMock = new Mock<IClientEventHistoryService>();
        var loggerMock = new Mock<ILogger<ClientWorkerService>>();
        var service = new ClientWorkerService(dbContext, historyMock.Object, loggerMock.Object);

        var client = CreateClient(EStatus.Active);
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        var message = CreateMessage(client, EOperation.Delete);
        dbContext.ClientMessages.Add(message);
        await dbContext.SaveChangesAsync();

        // Act
        await service.ConsumeAsync(CancellationToken.None);

        // Assert
        var dbClient = await dbContext.Clients.FindAsync(client.Id);
        Assert.Null(dbClient); // Removido
        var processedMessage = await dbContext.ClientMessages.FindAsync(message.Id);
        Assert.Equal(EMessageStatus.Processed, processedMessage.Status);
    }
}