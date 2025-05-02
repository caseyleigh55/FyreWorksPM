using FyreWorksPM.DataAccess.Data.Models;

namespace FyreWorksPM.Services.Client
{
    public interface IClientService
    {
        Task<List<ClientModel>> GetAllClientsAsync();
        Task<ClientModel?> GetClientByIdAsync(int id);
        Task AddClientAsync(ClientModel client);
        Task UpdateClientAsync(ClientModel client);
        Task DeleteClientAsync(int id);
    }
}
