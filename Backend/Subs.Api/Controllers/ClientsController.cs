using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subs.Core.Data;
using Subs.Core.Validators.SubscriptionComponents;
using Subs.Domain.Models.SubscriptionComponents;

namespace Subs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(SubsDbContext context) : ControllerBase
{
    private readonly SubsDbContext _context = context;

    /// <summary>
    /// Create a new Client
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Client>> Create([FromBody] Client client)
    {
        client.Id = Guid.NewGuid();

        var validator = new ClientValidator();
        var validationResult = await validator.ValidateAsync(client);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    /// <summary>
    /// Search client by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Client>> GetById(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is null)
            return NotFound();
        return Ok(client);
    }

    /// <summary>
    /// Search client by ID or create if not exists
    /// </summary>
    [HttpPost("get-or-create")]
    public async Task<ActionResult<Client>> GetOrCreate([FromBody] Client client)
    {
        var existing = await _context.Clients.FindAsync(client.Id);
        if (existing is not null)
            return Ok(existing);

        client.Id = client.Id == Guid.Empty ? Guid.NewGuid() : client.Id;
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    /// <summary>
    /// List all clients
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetAll()
    {
        var clients = await _context.Clients.ToListAsync();
        return Ok(clients);
    }

    /// <summary>
    /// Update client
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Client updated)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is null)
            return NotFound();

        client.FirstName = updated.FirstName;
        client.LastName = updated.LastName;
        client.Email = updated.Email;
        client.Phone = updated.Phone;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Delete client
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is null)
            return NotFound();

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}