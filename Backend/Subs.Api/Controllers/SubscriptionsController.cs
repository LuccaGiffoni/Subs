using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subs.Core.Data;
using Subs.Domain.DTOs;
using Subs.Domain.Enums;
using Subs.Domain.Interfaces.Entities;
using Subs.Domain.Models;

namespace Subs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController(SubsDbContext context,
                                     ISubscriptionService subscriptionService,
                                     IMapper mapper) : ControllerBase
{
    private readonly SubsDbContext _context = context;
    private readonly ISubscriptionService _subscriptionService = subscriptionService;
    private readonly IMapper _mapper = mapper;

    private const int DefaultPageSize = 10;
    private const int DefaultPage = 1;

    #region | Create
    /// <summary>
    /// Create a new subscription
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> Create([FromBody] SubscriptionDto dto)
    {
        var existingClient = await _context.Clients.FindAsync(dto.ClientId);
        if (existingClient is null)
            return BadRequest($"Client with Id {dto.ClientId} does not exist.");

        var subscription = _mapper.Map<Subscription>(dto);
        subscription.Client = existingClient;

        var result = await _subscriptionService.Create(subscription);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, _mapper.Map<SubscriptionDto>(result));
    }
    #endregion

    #region | Read
    /// <summary>
    /// Get all subscriptions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<object>> GetAll([FromQuery] int page = DefaultPage, [FromQuery] int pageSize = DefaultPageSize)
    {
        var query = _context.Subscriptions
            .Include(s => s.Client)
            .OrderByDescending(s => s.CreatedAt);

        var total = await query.CountAsync();
        var subscriptions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<SubscriptionDto>>(subscriptions);

        return Ok(new
        {
            total,
            page,
            pageSize,
            subscriptions = dtos
        });
    }

    /// <summary>
    /// Get subscription by Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SubscriptionDto>> GetById(Guid id)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Client)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subscription is null)
            return NotFound();

        return Ok(_mapper.Map<SubscriptionDto>(subscription));
    }

    /// <summary>
    /// Get all subscriptions for a given client
    /// </summary>
    [HttpGet("by-client/{clientId:guid}")]
    public async Task<ActionResult<List<SubscriptionDto>>> GetByClient(Guid clientId)
    {
        var subscriptions = await _context.Subscriptions
            .Where(s => s.Client!.Id == clientId)
            .Include(s => s.Client)
            .ToListAsync();

        var dtos = _mapper.Map<List<SubscriptionDto>>(subscriptions);

        return Ok(dtos);
    }

    /// <summary>
    /// Get all active subscriptions
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<List<SubscriptionDto>>> GetActive()
    {
        var subscriptions = await _context.Subscriptions
            .Where(s => s.Status == Domain.Enums.EStatus.Active)
            .Include(s => s.Client)
            .ToListAsync();

        var dtos = _mapper.Map<List<SubscriptionDto>>(subscriptions);

        return Ok(dtos);
    }

    #region | History
    /// <summary>
    /// Get paginated subscription history
    /// </summary>
    [HttpGet("{subscriptionId:guid}/history")]
    public async Task<ActionResult<object>> GetHistory(
        Guid subscriptionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
        if (subscription is null)
            return NotFound();

        var query = _context.SubscriptionsEventHistories
            .Where(h => h.SubscriptionId == subscriptionId)
            .OrderByDescending(h => h.CreatedAt);

        var total = await query.CountAsync();
        var history = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<SubscriptionEventHistoryDto>>(history);

        return Ok(new
        {
            total,
            page,
            pageSize,
            history = dtos
        });
    }
    #endregion
    #endregion

    #region | Update
    /// <summary>
    /// Update a subscription (e.g. status, payment details)
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SubscriptionDto dto)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription is null)
            return NotFound();

        var updated = _mapper.Map<Subscription>(dto);
        await _subscriptionService.Update(subscription, updated);

        return NoContent();
    }

    #region | Status Management
    /// <summary>
    /// Activate a subscription
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription is null)
            return NotFound();

        var updatedSubscription = await _subscriptionService.UpdateStatus(subscription, EStatus.Active);
        return Ok(_mapper.Map<SubscriptionDto>(updatedSubscription));
    }

    /// <summary>
    /// Suspend a subscription
    /// </summary>
    [HttpPost("{id:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription is null)
            return NotFound();

        var updatedSubscription = await _subscriptionService.UpdateStatus(subscription, EStatus.Suspended);
        return Ok(_mapper.Map<SubscriptionDto>(updatedSubscription));
    }

    /// <summary>
    /// Cancel a subscription
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription is null)
            return NotFound();

        var updatedSubscription = await _subscriptionService.UpdateStatus(subscription, EStatus.Canceled);
        return Ok(_mapper.Map<SubscriptionDto>(updatedSubscription));
    }
    #endregion
    #endregion

    #region | Delete
    /// <summary>
    /// Delete a subscription
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);

        if (subscription is null)
            return NotFound();

        await _subscriptionService.Delete(subscription);

        return NoContent();
    }
    #endregion
}