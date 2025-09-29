using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subs.Core.Data;
using Subs.Core.Validators;
using Subs.Domain.Enums;
using Subs.Domain.Models;
using Subs.Utils.Extensions;

namespace Subs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController(SubsDbContext context) : ControllerBase
{
    private readonly SubsDbContext _context = context;

    /// <summary>
    /// Create a new subscription
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Subscription>> Create([FromBody] Subscription subscription)
    {
        var clientId = subscription.Client.Id;
        var existingClient = await _context.Clients.FindAsync(clientId);

        if (existingClient is not null)
        {
            subscription.Client = existingClient;
        }
        else
        {
            subscription.Client.Id = clientId == Guid.Empty ? Guid.NewGuid() : clientId;
            _context.Clients.Add(subscription.Client);
            await _context.SaveChangesAsync();
        }

        subscription.Id = Guid.NewGuid();
        subscription.CreatedAt = subscription.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : subscription.CreatedAt.EnsureUtc();
        subscription.UpdatedAt = subscription.UpdatedAt == DateTime.MinValue ? DateTime.UtcNow : subscription.UpdatedAt.EnsureUtc();

        if (subscription.Payment?.Currency != null)
            subscription.Payment.Currency.Reference = subscription.Payment.Currency.Reference.EnsureUtc();

        var validator = new SubscriptionValidator();
        var validationResult = await validator.ValidateAsync(subscription);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
    }

    /// <summary>
    /// Get all subscriptions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Subscription>>> GetAll()
    {
        var subscriptions = await _context.Subscriptions
            .Include(s => s.Client)
            .ToListAsync();

        return Ok(subscriptions);
    }

    /// <summary>
    /// Get subscription by Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Subscription>> GetById(Guid id)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Client)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subscription is null)
            return NotFound();

        return Ok(subscription);
    }

    /// <summary>
    /// Update a subscription (e.g. status, payment details)
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Subscription updated)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);

        if (subscription is null)
            return NotFound();

        subscription.Status = updated.Status;
        subscription.UpdatedAt = DateTime.UtcNow;
        subscription.Payment = updated.Payment;
        subscription.Client = updated.Client;
        subscription.ProductId = updated.ProductId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Delete a subscription
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);

        if (subscription is null)
            return NotFound();

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Get all subscriptions for a given client
    /// </summary>
    [HttpGet("by-client/{clientId:guid}")]
    public async Task<ActionResult<IEnumerable<Subscription>>> GetByClient(Guid clientId)
    {
        var subscriptions = await _context.Subscriptions
            .Where(s => s.Client.Id == clientId)
            .Include(s => s.Client)
            .ToListAsync();

        return Ok(subscriptions);
    }

    /// <summary>
    /// Get all active subscriptions
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Subscription>>> GetActive()
    {
        var subscriptions = await _context.Subscriptions
            .Where(s => s.Status == Domain.Enums.EStatus.Active)
            .Include(s => s.Client)
            .ToListAsync();

        return Ok(subscriptions);
    }

    /// <summary>
    /// Activate a subscription
    /// </summary>
    /// <param name="id">Subscription's unique identifier</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription is null)
            return NotFound();

        subscription.Status = EStatus.Active;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(subscription);
    }

    /// <summary>
    /// Suspend a subscription
    /// </summary>
    /// <param name="id">Subscription's unique identifier</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription is null)
            return NotFound();

        subscription.Status = EStatus.Suspended;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(subscription);
    }

    /// <summary>
    /// Cancel a subscription
    /// </summary>
    /// <param name="id">Subscription's unique identifier</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription is null)
            return NotFound();

        subscription.Status = EStatus.Canceled;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(subscription);
    }
}
