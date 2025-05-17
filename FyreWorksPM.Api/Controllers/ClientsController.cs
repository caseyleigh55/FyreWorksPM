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
                ClientDtoId = c.ClientModelId,
                ClientDtoName = c.ClientModelName,
                ClientDtoContact = c.ClientModelContact,
                ClientDtoEmail = c.ClientModelEmail,
                ClientDtoPhone = c.ClientModelPhone
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
            ClientDtoId = client.ClientModelId,
            ClientDtoName = client.ClientModelName,
            ClientDtoContact = client.ClientModelContact,
            ClientDtoEmail = client.ClientModelEmail,
            ClientDtoPhone = client.ClientModelPhone
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
        if (string.IsNullOrWhiteSpace(dto.CreateClientDtoName) || string.IsNullOrWhiteSpace(dto.CreateClientDtoContact))
        {
            return BadRequest("Client name and contact are required.");
        }

        var client = new ClientModel
        {
            ClientModelName = dto.CreateClientDtoName,
            ClientModelContact = dto.CreateClientDtoContact,
            ClientModelEmail = dto.CreateClientDtoEmail,
            ClientModelPhone = dto.CreateClientDtoPhone
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        var createdDto = new ClientDto
        {
            ClientDtoId = client.ClientModelId,
            ClientDtoName = client.ClientModelName,
            ClientDtoContact = client.ClientModelContact,
            ClientDtoEmail = client.ClientModelEmail,
            ClientDtoPhone = client.ClientModelPhone
        };

        return CreatedAtAction(nameof(GetClient), new { id = client.ClientModelId }, new ClientDto
        {
            ClientDtoId = client.ClientModelId,
            ClientDtoName = client.ClientModelName,
            ClientDtoContact = client.ClientModelContact,
            ClientDtoEmail = client.ClientModelEmail,
            ClientDtoPhone = client.ClientModelPhone
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

        client.ClientModelName = dto.UpdateClientDtoName;
        client.ClientModelContact = dto.UpdateClientDtoContact;
        client.ClientModelEmail = dto.UpdateClientDtoEmail;
        client.ClientModelPhone = dto.UpdateClientDtoPhone;

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
