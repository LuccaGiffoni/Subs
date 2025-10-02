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

public class SubscriptionServiceTests
{
    private SubsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SubsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SubsDbContext(options);
    }

    private Subscription CreateSubscription(Guid? clientId = null)
    {
        var client = new Client
        {
            Id = clientId ?? Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@user.com",
            Phone = "5512991232566",
            Status = EStatus.Active
        };

        return new Subscription
        {
            Client = client,
            ClientId = client.Id,
            Payment = new Payment
            {
                Method = EPaymentMethod.Credit,
                Frequency = EPaymentFrequency.Monthly,
                Amount = 100,
                Currency = new Subs.Domain.Models.SubscriptionComponents.PaymentComponents.Currency { Type = "Real", Rate = 1.0m, Reference = DateTime.UtcNow },
                Discount = new Subs.Domain.Models.SubscriptionComponents.PaymentComponents.Discount { Type = EDiscountType.Percentage, Value = 0 }
            },
            Status = EStatus.Draft,
            ProductId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task Create_ShouldAddSubscriptionAndSendMessageAndAddEvents()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var busMock = new Mock<IBusService<SubscriptionMessage>>();
        var eventHistoryMock = new Mock<ISubscriptionEventHistoryService>();
        var loggerMock = new Mock<ILogger<SubscriptionService>>();
        var service = new SubscriptionService(dbContext, busMock.Object, eventHistoryMock.Object, loggerMock.Object);

        var client = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@user.com",
            Phone = "5512991232566",
            Status = EStatus.Active
        };
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        // Use a instância rastreada pelo contexto
        var trackedClient = await dbContext.Clients.FindAsync(client.Id);
        var subscription = CreateSubscription(trackedClient.Id);
        subscription.Client = trackedClient;

        // Act
        var result = await service.Create(subscription);

        // Assert
        var saved = await dbContext.Subscriptions.FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.NotNull(saved);
        eventHistoryMock.Verify(e => e.AddEvent(subscription, EOperation.Create, EStatus.Draft, It.IsAny<string>(), null), Times.Once);
        busMock.Verify(b => b.Send(It.IsAny<SubscriptionMessage>()), Times.Once);
        eventHistoryMock.Verify(e => e.AddEvent(subscription, EOperation.Create, EStatus.Pending, It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldModifySubscriptionAndSendMessageAndAddEvent()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var busMock = new Mock<IBusService<SubscriptionMessage>>();
        var eventHistoryMock = new Mock<ISubscriptionEventHistoryService>();
        var loggerMock = new Mock<ILogger<SubscriptionService>>();
        var service = new SubscriptionService(dbContext, busMock.Object, eventHistoryMock.Object, loggerMock.Object);

        var client = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@user.com",
            Phone = "5512991232566",
            Status = EStatus.Active
        };
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        // Use a instância rastreada pelo contexto
        var trackedClient = await dbContext.Clients.FindAsync(client.Id);
        var subscription = CreateSubscription(trackedClient.Id);
        subscription.Client = trackedClient;

        await dbContext.SaveChangesAsync();

        var updated = CreateSubscription(client.Id);
        updated.Status = EStatus.Active;
        updated.Payment.Amount = 200;

        // Act
        await service.Update(subscription, updated);

        // Assert
        Assert.Equal(EStatus.Active, subscription.Status);
        Assert.Equal(200, subscription.Payment.Amount);
        busMock.Verify(b => b.Send(It.IsAny<SubscriptionMessage>()), Times.Once);
        eventHistoryMock.Verify(e => e.AddEvent(subscription, EOperation.Update, subscription.Status, It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_ShouldChangeStatusAndSendMessageAndAddEvent()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var busMock = new Mock<IBusService<SubscriptionMessage>>();
        var eventHistoryMock = new Mock<ISubscriptionEventHistoryService>();
        var loggerMock = new Mock<ILogger<SubscriptionService>>();
        var service = new SubscriptionService(dbContext, busMock.Object, eventHistoryMock.Object, loggerMock.Object);

        var client = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@user.com",
            Phone = "5512991232566",
            Status = EStatus.Active
        };
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        var trackedClient = await dbContext.Clients.FindAsync(client.Id);
        var subscription = CreateSubscription(trackedClient.Id);
        subscription.Client = trackedClient;

        dbContext.Subscriptions.Add(subscription);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.UpdateStatus(subscription, EStatus.Suspended);

        // Assert
        Assert.Equal(EStatus.Suspended, subscription.Status);
        busMock.Verify(b => b.Send(It.IsAny<SubscriptionMessage>()), Times.Once);
        eventHistoryMock.Verify(e => e.AddEvent(subscription, EOperation.Update, subscription.Status, It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldSendMessageAndAddEvent()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var busMock = new Mock<IBusService<SubscriptionMessage>>();
        var eventHistoryMock = new Mock<ISubscriptionEventHistoryService>();
        var loggerMock = new Mock<ILogger<SubscriptionService>>();
        var service = new SubscriptionService(dbContext, busMock.Object, eventHistoryMock.Object, loggerMock.Object);

        var client = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@user.com",
            Phone = "5512991232566",
            Status = EStatus.Active
        };
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        var trackedClient = await dbContext.Clients.FindAsync(client.Id);
        var subscription = CreateSubscription(trackedClient.Id);
        subscription.Client = trackedClient;

        dbContext.Subscriptions.Add(subscription);
        await dbContext.SaveChangesAsync();

        // Act
        await service.Delete(subscription);

        // Assert
        busMock.Verify(b => b.Send(It.IsAny<SubscriptionMessage>()), Times.Once);
        eventHistoryMock.Verify(e => e.AddEvent(subscription, EOperation.Delete, subscription.Status, It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldThrowArgumentException_WhenClientIsInactive()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var busMock = new Mock<IBusService<SubscriptionMessage>>();
        var eventHistoryMock = new Mock<ISubscriptionEventHistoryService>();
        var loggerMock = new Mock<ILogger<SubscriptionService>>();
        var service = new SubscriptionService(dbContext, busMock.Object, eventHistoryMock.Object, loggerMock.Object);

        var client = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@user.com",
            Phone = "5512991232566",
            Status = EStatus.Canceled
        };
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        var subscription = CreateSubscription(client.Id);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.Create(subscription));
    }
}