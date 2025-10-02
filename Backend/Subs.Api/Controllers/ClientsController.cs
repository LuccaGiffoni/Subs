using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subs.Core.Data;
using Subs.Domain.DTOs;
using Subs.Domain.Interfaces.Entities;
using Subs.Domain.Models.SubscriptionComponents;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(SubsDbContext context,
                               IClientService clientService,
                               IMapper mapper) : ControllerBase
{
    private readonly SubsDbContext _context = context;
    private readonly IClientService _clientService = clientService;
    private readonly IMapper _mapper = mapper;

    private const int DefaultPageSize = 10;
    private const int DefaultPage = 1;

    #region | Create
    /// <summary>
    /// Create a new Client
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create([FromBody] ClientDto dto)
    {
        var client = _mapper.Map<Client>(dto);
        var result = await _clientService.Create(client);
        var resultDto = _mapper.Map<ClientDto>(result);

        return CreatedAtAction(nameof(GetById), new { id = resultDto.Id }, resultDto);
    }
    #endregion

    #region | Read
    /// <summary>
    /// Search client by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientDto>> GetById(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is null)
            return NotFound();
        return Ok(_mapper.Map<ClientDto>(client));
    }

    /// <summary>
    /// List all clients
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<object>> GetAll(
        [FromQuery] int page = DefaultPage,
        [FromQuery] int pageSize = DefaultPageSize,
        [FromQuery] string? search = null)
    {
        var query = _context.Clients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowerSearch = search.ToLower();
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(lowerSearch) ||
                c.LastName.ToLower().Contains(lowerSearch) ||
                c.Email.ToLower().Contains(lowerSearch));
        }

        query = query.OrderByDescending(s => s.FirstName).ThenByDescending(s => s.LastName);

        var total = await query.CountAsync();
        var clients = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<ClientDto>>(clients);

        return Ok(new
        {
            total,
            page,
            pageSize,
            clients = dtos
        });
    }

    #region | History
    /// <summary>
    /// Get paginated client history
    /// </summary>
    [HttpGet("{clientId:guid}/history")]
    public async Task<ActionResult<object>> GetHistory(
        Guid clientId,
        [FromQuery] int page = DefaultPage,
        [FromQuery] int pageSize = DefaultPageSize)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client is null)
            return NotFound();

        var query = _context.ClientEventHistories
            .Where(h => h.ClientId == clientId)
            .OrderByDescending(h => h.CreatedAt);

        var total = await query.CountAsync();
        var history = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<ClientEventHistoryDto>>(history);

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
    /// Update client
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ClientDto dto)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is null)
            return NotFound();

        var updated = _mapper.Map<Client>(dto);
        await _clientService.Update(client, updated);
        return NoContent();
    }
    #endregion

    #region | Delete
    /// <summary>
    /// Delete client
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is null)
            return NotFound();

        await _clientService.Delete(client);
        return NoContent();
    }
    #endregion
}