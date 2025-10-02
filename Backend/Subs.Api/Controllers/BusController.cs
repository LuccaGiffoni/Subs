using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subs.Core.Data;
using Subs.Domain.DTOs;
using Subs.Domain.Enums;

namespace Subs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusController(SubsDbContext dbContext, IMapper mapper) : ControllerBase
{
    private readonly SubsDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;

    private const int DefaultPageSize = 10;
    private const int DefaultPage = 1;

    #region | Read
    /// <summary>
    /// Gets paginated messages from the ClientMessages queue
    /// </summary>
    [HttpGet("clients")]
    public async Task<ActionResult<object>> GetClientQueueMessages(
        [FromQuery] int page = DefaultPage,
        [FromQuery] int pageSize = DefaultPageSize)
    {
        var clientMessages = await _dbContext.ClientMessages
            .AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var total = await _dbContext.ClientMessages.CountAsync();
        var dtos = _mapper.Map<List<ClientMessageDto>>(clientMessages);

        return Ok(new
        {
            total,
            page,
            pageSize,
            items = dtos
        });
    }

    /// <summary>
    /// Gets paginated messages from the SubscriptionMessages queue
    /// </summary>
    [HttpGet("subscriptions")]
    public async Task<ActionResult<object>> GetSubscriptionQueueMessages(
        [FromQuery] int page = DefaultPage,
        [FromQuery] int pageSize = DefaultPageSize)
    {
        var subscriptionMessages = await _dbContext.SubscriptionMessages
            .AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var total = await _dbContext.SubscriptionMessages.CountAsync();
        var dtos = _mapper.Map<List<SubscriptionMessageDto>>(subscriptionMessages);

        return Ok(new
        {
            total,
            page,
            pageSize,
            items = dtos
        });
    }
    #endregion

    #region | Delete

    /// <summary>
    /// Purge all pending ClientMessages (Draft or Pending status)
    /// </summary>
    [HttpDelete("clients/purge")]
    public async Task<IActionResult> PurgePendingClientMessages()
    {
        var pendingStatuses = new[] { EMessageStatus.Received, EMessageStatus.Processing };

        var messagesToDelete = await _dbContext.ClientMessages
            .Where(m => pendingStatuses.Contains(m.Status))
            .ToListAsync();

        if (messagesToDelete.Count == 0)
            return NoContent();

        _dbContext.ClientMessages.RemoveRange(messagesToDelete);
        await _dbContext.SaveChangesAsync();

        return Ok(new { deleted = messagesToDelete.Count });
    }

    /// <summary>
    /// Purge all pending SubscriptionMessages (Draft or Pending status)
    /// </summary>
    [HttpDelete("subscriptions/purge")]
    public async Task<IActionResult> PurgePendingSubscriptionMessages()
    {
        var pendingStatuses = new[] { EMessageStatus.Received, EMessageStatus.Processing };

        var messagesToDelete = await _dbContext.SubscriptionMessages
            .Where(m => pendingStatuses.Contains(m.Status))
            .ToListAsync();

        if (messagesToDelete.Count == 0)
            return NoContent();

        _dbContext.SubscriptionMessages.RemoveRange(messagesToDelete);
        await _dbContext.SaveChangesAsync();

        return Ok(new { deleted = messagesToDelete.Count });
    }

    #endregion
}