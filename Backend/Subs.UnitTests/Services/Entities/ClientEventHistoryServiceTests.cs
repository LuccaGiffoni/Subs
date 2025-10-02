using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Subs.Core.Data;
using Subs.Core.Services.Entities;
using Subs.Domain.Enums;
using Subs.Domain.Models;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.UnitTests.Services.Entities;

public class ClientEventHistoryServiceTests
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
            Phone = "123456789",
            Status = EStatus.Active
        };
    }

    [Fact]
    public async Task AddEvent_ShouldAddHistoryToDatabase()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = new ClientEventHistoryService(dbContext);
        var client = CreateClient();

        // Act
        await service.AddEvent(client, EOperation.Create, EStatus.Active, "Created client", null);

        // Assert
        var history = await dbContext.ClientEventHistories.FirstOrDefaultAsync(h => h.ClientId == client.Id);
        Assert.NotNull(history);
        Assert.Equal(client.Id, history.ClientId);
        Assert.Equal(EOperation.Create, history.Operation);
        Assert.Equal(EStatus.Active, history.StatusAtEvent);
        Assert.Equal("Created client", history.Note);
        Assert.True(history.CreatedAt <= DateTime.UtcNow);
        Assert.Equal(Guid.Empty, history.RollbackId);
    }

    [Fact]
    public async Task AddEvent_ShouldSetRollbackId_WhenProvided()
    {
        // Arrange
        var dbContext = CreateDbContext();
        var service = new ClientEventHistoryService(dbContext);
        var client = CreateClient();
        var rollbackId = Guid.NewGuid();

        // Act
        await service.AddEvent(client, EOperation.Rollback, EStatus.Suspended, "Rollback event", rollbackId);

        // Assert
        var history = await dbContext.ClientEventHistories.FirstOrDefaultAsync(h => h.ClientId == client.Id && h.Operation == EOperation.Rollback);
        Assert.NotNull(history);
        Assert.Equal(rollbackId, history.RollbackId);
        Assert.Equal(EStatus.Suspended, history.StatusAtEvent);
        Assert.Equal("Rollback event", history.Note);
    }
}