using System.Net.Http.Json;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Services.Item;

/// <summary>
/// API-based implementation of IItemTypeService using HttpClient.
/// </summary>
public class ItemTypeService : IItemTypeService
{
    private readonly HttpClient _http;

    public ItemTypeService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Returns all item type DTOs from the API.
    /// </summary>
    public async Task<List<ItemTypeDto>> GetAllItemTypesAsync()
    {
        var response = await _http.GetAsync("api/itemtypes");
        response.EnsureSuccessStatusCode();

        var types = await response.Content.ReadFromJsonAsync<List<ItemTypeDto>>();
        return types ?? new List<ItemTypeDto>();
    }

    /// <summary>
    /// Returns just the names of all item types.
    /// </summary>
    public async Task<List<string>> GetAllItemTypeNamesAsync()
    {
        var types = await GetAllItemTypesAsync();
        return types.Select(t => t.Name).Distinct().ToList();
    }

    /// <summary>
    /// Gets one item type by ID.
    /// </summary>
    public async Task<ItemTypeDto?> GetItemTypeByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/itemtypes/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ItemTypeDto>();
    }

    /// <summary>
    /// Adds a new item type via API.
    /// </summary>
    public async Task AddItemTypeAsync(ItemTypeDto itemType)
    {
        var dto = new CreateItemTypeDto { Name = itemType.Name };
        var response = await _http.PostAsJsonAsync("api/itemtypes", dto);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Updates an item type by ID.
    /// </summary>
    public async Task UpdateItemTypeAsync(ItemTypeDto itemType)
    {
        var dto = new CreateItemTypeDto { Name = itemType.Name };
        var response = await _http.PutAsJsonAsync($"api/itemtypes/{itemType.Id}", dto);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deletes an item type by ID.
    /// </summary>
    public async Task DeleteItemTypeAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/itemtypes/{id}");
        response.EnsureSuccessStatusCode();
    }

    public Task AddItemTypeAsync(ItemTypeModel itemType)
    {
        throw new NotImplementedException();
    }

    public Task UpdateItemTypeAsync(ItemTypeModel itemType)
    {
        throw new NotImplementedException();
    }
}
