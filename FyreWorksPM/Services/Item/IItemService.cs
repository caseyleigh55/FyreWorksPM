using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Services.Item;

/// <summary>
/// Contract for interacting with item-related data via API.
/// </summary>
public interface IItemService
{
    /// <summary>
    /// Gets all items.
    /// </summary>
    Task<List<ItemDto>> GetAllItemsAsync();

    /// <summary>
    /// Gets a specific item by ID.
    /// </summary>
    Task<ItemDto?> GetItemByIdAsync(int id);

    /// <summary>
    /// Adds a new item.
    /// </summary>
    Task AddItemAsync(CreateItemDto dto);

    /// <summary>
    /// Updates an existing item by ID.
    /// </summary>
    Task UpdateItemAsync(int id, CreateItemDto dto);

    /// <summary>
    /// Deletes an item by ID.
    /// </summary>
    Task DeleteItemAsync(int id);
}
