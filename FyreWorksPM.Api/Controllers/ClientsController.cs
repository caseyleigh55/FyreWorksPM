using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ClientsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ================================================
    // ✅ GET: api/clients
    // Returns all clients
    // ================================================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetAllClients()
    {
        var clients = await _db.Clients
            .Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                Contact = c.Contact,
                Email = c.Email,
                Phone = c.Phone
            })
            .ToListAsync();

        return Ok(clients);
    }

    // ================================================
    // ✅ GET: api/clients/{id}
    // Returns a single client by ID
    // ================================================
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null) return NotFound();

        var dto = new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Contact = client.Contact,
            Email = client.Email,
            Phone = client.Phone
        };

        return Ok(dto);
    }

    // ================================================
    // ✅ POST: api/clients
    // Adds a new client
    // ================================================
    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient([FromBody] CreateClientDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Contact))
        {
            return BadRequest("Client name and contact are required.");
        }

        var client = new ClientModel
        {
            Name = dto.Name,
            Contact = dto.Contact,
            Email = dto.Email,
            Phone = dto.Phone
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        var createdDto = new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Contact = client.Contact,
            Email = client.Email,
            Phone = client.Phone
        };

        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            Contact = client.Contact,
            Email = client.Email,
            Phone = client.Phone
        });
    }

    // ================================================
    // ✅ PUT: api/clients/{id}
    // Updates an existing client
    // ================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(int id, [FromBody] UpdateClientDto dto)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null) return NotFound();

        client.Name = dto.Name;
        client.Contact = dto.Contact;
        client.Email = dto.Email;
        client.Phone = dto.Phone;

        _db.Clients.Update(client);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // ================================================
    // ✅ DELETE: api/clients/{id}
    // Deletes a client
    // ================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null) return NotFound();

        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
