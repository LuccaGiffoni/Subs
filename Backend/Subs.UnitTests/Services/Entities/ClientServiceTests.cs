using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Subs.Core.Data;
using Subs.Core.Services.Entities;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.Bus;
using Subs.Domain.Interfaces.History;
using Subs.Domain.Models;
using Subs.Domain.Models.Messages;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.UnitTests.Services.Entities;

public class ClientServiceTests
{
    private SubsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SubsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SubsDbContext(options);
    }

    private Client CreateClient()
    {
        return new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@user.com",
            Phone = "5512991232566",
            Status = EStatus.Active
        };
    }

    [Fact]
    public async Task Create_ShouldAddClientAndSendMessageAndAddEvents()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var busMock = new Mock<IBusService<ClientMessage>>();
        var eventHistoryMock = new Mock<IClientEventHistoryService>();
        var loggerMock = new Mock<ILogger<ClientService>>();
        var service = new ClientService(dbContext, busMock.Object, eventHistoryMock.Object, loggerMock.Object);
        var client = CreateClient();

        // Act
        var result = await service.Create(client);

        // Assert
        var saved = await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == client.Id);
        Assert.NotNull(saved);
        eventHistoryMock.Verify(e => e.AddEvent(client, EOperation.Create, EStatus.Draft, It.IsAny<string>(), null), Times.Once);
        busMock.Verify(b => b.Send(It.IsAny<ClientMessage>()), Times.Once);
        eventHistoryMock.Verify(e => e.AddEvent(client, EOperation.Create, EStatus.Pending, It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldModifyClientAndSendMessageAndAddEvent()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var busMock = new Mock<IBusService<ClientMessage>>();
        var eventHistoryMock = new Mock<IClientEventHistoryService>();
        var loggerMock = new Mock<ILogger<ClientService>>();
        var service = new ClientService(dbContext, busMock.Object, eventHistoryMock.Object, loggerMock.Object);
        var client = CreateClient();
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        var updated = new Client
        {
            FirstName = "Updated",
            LastName = "User",
            Email = "updated@user.com",
            Phone = "5512991232566",
            Status = EStatus.Active
        };

        // Act
        var result = await service.Update(client, updated);

        // Assert
        Assert.Equal("Updated", client.FirstName);
        Assert.Equal("updated@user.com", client.Email);
        busMock.Verify(b => b.Send(It.IsAny<ClientMessage>()), Times.Once);
        eventHistoryMock.Verify(e => e.AddEvent(client, EOperation.Update, client.Status, It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldSendMessageAndAddEvent()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var busMock = new Mock<IBusService<ClientMessage>>();
        var eventHistoryMock = new Mock<IClientEventHistoryService>();
        var loggerMock = new Mock<ILogger<ClientService>>();
        var service = new ClientService(dbContext, busMock.Object, eventHistoryMock.Object, loggerMock.Object);
        var client = CreateClient();
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        // Act
        await service.Delete(client);

        // Assert
        busMock.Verify(b => b.Send(It.IsAny<ClientMessage>()), Times.Once);
        eventHistoryMock.Verify(e => e.AddEvent(client, EOperation.Delete, client.Status, It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldThrowArgumentException_WhenValidationFails()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var busMock = new Mock<IBusService<ClientMessage>>();
        var eventHistoryMock = new Mock<IClientEventHistoryService>();
        var loggerMock = new Mock<ILogger<ClientService>>();
        var service = new ClientService(dbContext, busMock.Object, eventHistoryMock.Object, loggerMock.Object);
        var client = new Client(); // Campos obrigatórios não preenchidos

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.Create(client));
    }
}