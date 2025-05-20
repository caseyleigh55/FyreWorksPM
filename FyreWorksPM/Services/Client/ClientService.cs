using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json;

namespace FyreWorksPM.Services.Client;

/// <summary>
/// Service responsible for managing CRUD operations on clients.
/// This class interacts with the database via EF Core.
/// </summary>
public class ClientService : IClientService
{
    private readonly HttpClient _httpClient;

    public ClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Retrieves all clients from the database.
    /// </summary>
    public async Task<List<ClientDto>> GetAllClientsAsync()
    {
        var response = await _httpClient.GetAsync("api/clients");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ClientDto>>();
    }

    /// <summary>
    /// Retrieves a single client by their unique ID.
    /// Returns null if no match is found.
    /// </summary>
    public async Task<ClientDto?> GetClientByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/clients/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ClientDto>();
    }

    public async Task<ClientDto?> AddClientAsync(CreateClientDto client)
    {
        var response = await _httpClient.PostAsJsonAsync("api/clients", client);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            {
                // ⚠️ Known client-side issue (validation, already exists, etc.)
                await Application.Current.MainPage.DisplayAlert("Client Error", content, "OK");
                return null;
            }

            // 🚨 Unexpected or server-side error
            throw new Exception($"❌ API Error: {response.StatusCode}\nBody:\n{content}");
        }

        // ✅ Successful response, parse the JSON
        var json = await response.Content.ReadAsStringAsync();

        try
        {
            return JsonSerializer.Deserialize<ClientDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            throw new Exception($"⚠️ JSON parse error.\nResponse Body:\n{json}", ex);
        }
    }


    /// <summary>
    /// Updates an existing client's information in the database.
    /// </summary>
    /// <param name="client">The client with updated values.</param>
    public async Task UpdateClientAsync(int id, CreateClientDto client)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/clients/{id}", client);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deletes a client from the database by ID.
    /// </summary>
    /// <param name="id">The ID of the client to delete.</param>
    public async Task DeleteClientAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/clients/{id}");
        response.EnsureSuccessStatusCode();
    }
}
