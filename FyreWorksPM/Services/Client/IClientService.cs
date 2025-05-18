using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Services.Client
{
    /// <summary>
    /// Defines the operations available for managing Client data.
    /// Used by services that interact with the database.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Retrieves all clients from the data source.
        /// </summary>
        Task<List<ClientDto>> GetAllClientsAsync();

        /// <summary>
        /// Retrieves a single client by their unique ID.
        /// </summary>
        /// <param name="id">The client's ID.</param>
        Task<ClientDto?> GetClientByIdAsync(int id);

        /// <summary>
        /// Adds a new client to the data source.
        /// </summary>
        /// <param name="client">The client to add.</param>
        Task<ClientDto> AddClientAsync(CreateClientDto client);

        /// <summary>
        /// Updates an existing client's information.
        /// </summary>
        /// <param name="client">The client with updated info.</param>
        Task UpdateClientAsync(int id, CreateClientDto client);

        /// <summary>
        /// Deletes a client by ID.
        /// </summary>
        /// <param name="id">The ID of the client to delete.</param>
        Task DeleteClientAsync(int id);
    }
}
