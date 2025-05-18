using System.Net.Http.Json;
using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Services.Item;

/// <summary>
/// API-based implementation of IItemService using HttpClient.
/// </summary>
public class ItemService : IItemService
{
    private readonly HttpClient _http;

    public ItemService(HttpClient http)
    {        
        _http = http;
    }

    /// <summary>
    /// Gets all items from the API with their item type name.
    /// </summary>
    public async Task<List<ItemDto>> GetAllItemsAsync()
    {
        var response = await _http.GetAsync("api/items");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<ItemDto>>();
        return items ?? new List<ItemDto>();
    }

    /// <summary>
    /// Gets a single item by ID from the API.
    /// </summary>
    public async Task<ItemDto?> GetItemByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/items/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ItemDto>();
    }

    /// <summary>
    /// Adds a new item via the API.
    /// </summary>
    public async Task AddItemAsync(CreateItemDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/items", dto);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Updates an existing item by ID via the API.
    /// </summary>
    public async Task UpdateItemAsync(int id, CreateItemDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/items/{id}", dto);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deletes an item by ID via the API.
    /// </summary>
    public async Task DeleteItemAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/items/{id}");
        response.EnsureSuccessStatusCode();    
    }
}
