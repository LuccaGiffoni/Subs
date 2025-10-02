using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Subs.Core.Data;
using Subs.Core.Services.Entities;
using Subs.Domain.Enums;
using Subs.Domain.Models;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.UnitTests.Services.Entities;

public class SubscriptionEventHistoryServiceTests
{
    private SubsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SubsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SubsDbContext(options);
    }

    private Subscription CreateSubscription()
    {
        return new Subscription
        {
            Id = Guid.NewGuid(),
            Client = new Client { Id = Guid.NewGuid() },
            Payment = new Payment(),
            Status = EStatus.Active,
            ProductId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task AddEvent_ShouldAddHistoryWithCorrectFields()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var loggerMock = new Mock<ILogger<SubscriptionEventHistoryService>>();
        var service = new SubscriptionEventHistoryService(dbContext, loggerMock.Object);

        var subscription = CreateSubscription();
        var operation = EOperation.Create;
        var status = EStatus.Active;
        var note = "Test note";
        var rollbackId = Guid.NewGuid();

        // Act
        await service.AddEvent(subscription, operation, status, note, rollbackId);
        await dbContext.SaveChangesAsync();

        // Assert
        var history = await dbContext.SubscriptionsEventHistories.FirstOrDefaultAsync(h =>
            h.SubscriptionId == subscription.Id &&
            h.Operation == operation &&
            h.StatusAtEvent == status &&
            h.Note == note &&
            h.RollbackId == rollbackId
        );
        Assert.NotNull(history);
    }

    [Fact]
    public async Task AddEvent_ShouldSetRollbackIdToEmpty_WhenNull()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var loggerMock = new Mock<ILogger<SubscriptionEventHistoryService>>();
        var service = new SubscriptionEventHistoryService(dbContext, loggerMock.Object);

        var subscription = CreateSubscription();
        var operation = EOperation.Delete;
        var status = EStatus.Canceled;
        var note = "No rollback";

        // Act
        await service.AddEvent(subscription, operation, status, note, null);
        await dbContext.SaveChangesAsync();

        // Assert
        var history = await dbContext.SubscriptionsEventHistories.FirstOrDefaultAsync(h =>
            h.SubscriptionId == subscription.Id &&
            h.Operation == operation &&
            h.StatusAtEvent == status &&
            h.Note == note &&
            h.RollbackId == Guid.Empty
        );
        Assert.NotNull(history);
    }
}