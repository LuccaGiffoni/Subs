using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql.Internal;
using Subs.Core.Data;
using Subs.Core.Services.Worker;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.History;
using Subs.Domain.Models;
using Subs.Domain.Models.Messages;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.UnitTests.Services.Worker;

public class SubscriptionWorkerServiceTests
{
    private SubsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SubsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SubsDbContext(options);
    }

    private Subscription CreateSubscription(Guid clientId, EStatus status = EStatus.Draft)
    {
        return new Subscription
        {
            Id = Guid.NewGuid(),
            Client = new(),
            ClientId = clientId,
            Status = status,
            ProductId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Payment = new Payment
            {
                Method = EPaymentMethod.Credit,
                Frequency = EPaymentFrequency.Monthly,
                Amount = 100,
                Currency = new Subs.Domain.Models.SubscriptionComponents.PaymentComponents.Currency { Type = "Real", Rate = 1.0m, Reference = DateTime.UtcNow },
                Discount = new Subs.Domain.Models.SubscriptionComponents.PaymentComponents.Discount { Type = EDiscountType.Percentage, Value = 0 }
            }
        };
    }

    private SubscriptionMessage CreateMessage(Guid subscriptionId, EOperation operation)
    {
        return new SubscriptionMessage(subscriptionId, operation)
        {
            Status = EMessageStatus.Received,
            CallbackAt = DateTime.UtcNow.AddSeconds(-1)
        };
    }

    [Fact]
    public async Task ConsumeAsync_ShouldActivateSubscription_OnCreateOperation()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var historyMock = new Mock<ISubscriptionEventHistoryService>();
        var loggerMock = new Mock<ILogger<SubscriptionWorkerService>>();
        var service = new SubscriptionWorkerService(dbContext, historyMock.Object, loggerMock.Object);

        var client = new Client { Id = Guid.NewGuid(), Status = EStatus.Active };
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        var subscription = CreateSubscription(client.Id);
        dbContext.Subscriptions.Add(subscription);
        await dbContext.SaveChangesAsync();

        var message = CreateMessage(subscription.Id, EOperation.Create);
        dbContext.SubscriptionMessages.Add(message);
        await dbContext.SaveChangesAsync();

        // Act
        await service.ConsumeAsync(CancellationToken.None);

        // Assert
        var updatedSubscription = await dbContext.Subscriptions.FindAsync(subscription.Id);
        Assert.Equal(EStatus.Active, updatedSubscription.Status);
        historyMock.Verify(h => h.AddEvent(updatedSubscription, EOperation.Create, EStatus.Active, It.IsAny<string>(), null), Times.Once);
        var processedMessage = await dbContext.SubscriptionMessages.FindAsync(message.Id);
        Assert.Equal(EMessageStatus.Processed, processedMessage.Status);
    }

    [Fact]
    public async Task ConsumeAsync_ShouldUpdateSubscription_OnUpdateOperation()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var historyMock = new Mock<ISubscriptionEventHistoryService>();
        var loggerMock = new Mock<ILogger<SubscriptionWorkerService>>();
        var service = new SubscriptionWorkerService(dbContext, historyMock.Object, loggerMock.Object);

        var client = new Client { Id = Guid.NewGuid(), Status = EStatus.Active };
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        var subscription = CreateSubscription(client.Id, EStatus.Active);
        dbContext.Subscriptions.Add(subscription);
        await dbContext.SaveChangesAsync();

        var updatedSubscription = CreateSubscription(client.Id, EStatus.Suspended);
        updatedSubscription.Id = subscription.Id;
        updatedSubscription.Payment.Amount = 200;

        var message = CreateMessage(subscription.Id, EOperation.Update);
        dbContext.SubscriptionMessages.Add(message);
        await dbContext.SaveChangesAsync();

        // Simula atualização
        subscription.Status = updatedSubscription.Status;
        subscription.Payment = updatedSubscription.Payment;
        subscription.ProductId = updatedSubscription.ProductId;

        // Act
        await service.ConsumeAsync(CancellationToken.None);

        // Assert
        var dbSub = await dbContext.Subscriptions.FindAsync(subscription.Id);
        Assert.Equal(EStatus.Suspended, dbSub.Status);
        Assert.Equal(200, dbSub.Payment.Amount);
        historyMock.Verify(h => h.AddEvent(dbSub, EOperation.Update, dbSub.Status, It.IsAny<string>(), null), Times.Once);
        var processedMessage = await dbContext.SubscriptionMessages.FindAsync(message.Id);
        Assert.Equal(EMessageStatus.Processed, processedMessage.Status);
    }

    [Fact]
    public async Task ConsumeAsync_ShouldDeleteSubscription_OnDeleteOperation()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var historyMock = new Mock<ISubscriptionEventHistoryService>();
        var loggerMock = new Mock<ILogger<SubscriptionWorkerService>>();
        var service = new SubscriptionWorkerService(dbContext, historyMock.Object, loggerMock.Object);

        var client = new Client { Id = Guid.NewGuid(), Status = EStatus.Active };
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        var subscription = CreateSubscription(client.Id, EStatus.Active);
        dbContext.Subscriptions.Add(subscription);
        await dbContext.SaveChangesAsync();

        var message = CreateMessage(subscription.Id, EOperation.Delete);
        dbContext.SubscriptionMessages.Add(message);
        await dbContext.SaveChangesAsync();

        // Act
        await service.ConsumeAsync(CancellationToken.None);

        // Assert
        var dbSub = await dbContext.Subscriptions.FindAsync(subscription.Id);
        Assert.Null(dbSub); // Removido
        var processedMessage = await dbContext.SubscriptionMessages.FindAsync(message.Id);
        Assert.Equal(EMessageStatus.Processed, processedMessage.Status);
    }

    [Fact]
    public async Task ConsumeAsync_ShouldFail_WhenSubscriptionNotFound()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var historyMock = new Mock<ISubscriptionEventHistoryService>();
        var loggerMock = new Mock<ILogger<SubscriptionWorkerService>>();
        var service = new SubscriptionWorkerService(dbContext, historyMock.Object, loggerMock.Object);

        var message = CreateMessage(Guid.NewGuid(), EOperation.Create);
        dbContext.SubscriptionMessages.Add(message);
        await dbContext.SaveChangesAsync();

        // Act
        await service.ConsumeAsync(CancellationToken.None);

        // Assert
        var processedMessage = await dbContext.SubscriptionMessages.FindAsync(message.Id);
        Assert.Equal(EMessageStatus.Failed, processedMessage.Status);
    }

    [Fact]
    public async Task ConsumeAsync_ShouldFail_WhenClientNotFound()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var historyMock = new Mock<ISubscriptionEventHistoryService>();
        var loggerMock = new Mock<ILogger<SubscriptionWorkerService>>();
        var service = new SubscriptionWorkerService(dbContext, historyMock.Object, loggerMock.Object);

        var subscription = CreateSubscription(Guid.NewGuid(), EStatus.Active);
        dbContext.Subscriptions.Add(subscription);
        await dbContext.SaveChangesAsync();

        var message = CreateMessage(subscription.Id, EOperation.Create);
        dbContext.SubscriptionMessages.Add(message);
        await dbContext.SaveChangesAsync();

        // Remove o client
        var client = await dbContext.Clients.FindAsync(subscription.ClientId);
        if (client != null)
        {
            dbContext.Clients.Remove(client);
            await dbContext.SaveChangesAsync();
        }

        // Act
        await service.ConsumeAsync(CancellationToken.None);

        // Assert
        var processedMessage = await dbContext.SubscriptionMessages.FindAsync(message.Id);
        Assert.Equal(EMessageStatus.Failed, processedMessage.Status);
    }
}