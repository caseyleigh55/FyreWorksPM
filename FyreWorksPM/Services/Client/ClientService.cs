using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FyreWorksPM.Services.Client;

public class ClientService : IClientService
{
    private readonly ApplicationDbContext _db;

    public ClientService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ClientModel>> GetAllClientsAsync()
    {
        return await _db.Clients.ToListAsync();
    }

    public async Task<ClientModel?> GetClientByIdAsync(int id)
    {
        return await _db.Clients.FindAsync(id);
    }

    public async Task AddClientAsync(ClientModel client)
    {
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateClientAsync(ClientModel client)
    {
        _db.Clients.Update(client);
        await _db.SaveChangesAsync();
    }

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
