using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FyreWorksPM.Services.Client;

/// <summary>
/// Service responsible for managing CRUD operations on clients.
/// This class interacts with the database via EF Core.
/// </summary>
public class ClientService : IClientService
{
    private readonly ApplicationDbContext _db;

    public ClientService(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves all clients from the database.
    /// </summary>
    public async Task<List<ClientModel>> GetAllClientsAsync()
    {
        return await _db.Clients.ToListAsync();
    }

    /// <summary>
    /// Retrieves a single client by their unique ID.
    /// Returns null if no match is found.
    /// </summary>
    public async Task<ClientModel?> GetClientByIdAsync(int id)
    {
        return await _db.Clients.FindAsync(id);
    }

    /// <summary>
    /// Adds a new client to the database.
    /// </summary>
    /// <param name="client">The client to be added.</param>
    public async Task AddClientAsync(ClientModel client)
    {
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing client's information in the database.
    /// </summary>
    /// <param name="client">The client with updated values.</param>
    public async Task UpdateClientAsync(ClientModel client)
    {
        _db.Clients.Update(client);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a client from the database by ID.
    /// </summary>
    /// <param name="id">The ID of the client to delete.</param>
    public async Task DeleteClientAsync(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client != null)
        {
            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();
        }
    }
}
