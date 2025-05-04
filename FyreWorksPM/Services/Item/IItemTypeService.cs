using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Services.Item;

/// <summary>
/// Contract for interacting with item types via API.
/// </summary>
public interface IItemTypeService
{
    /// <summary>
    /// Gets all item types.
    /// </summary>
    Task<List<ItemTypeDto>> GetAllItemTypesAsync();

    /// <summary>
    /// Gets a list of item type names.
    /// </summary>
    Task<List<string>> GetAllItemTypeNamesAsync();

    /// <summary>
    /// Gets a single item type by ID.
    /// </summary>
    Task<ItemTypeDto?> GetItemTypeByIdAsync(int id);

    /// <summary>
    /// Adds a new item type.
    /// </summary>
    Task AddItemTypeAsync(ItemTypeDto itemType);

    /// <summary>
    /// Updates an existing item type.
    /// </summary>
    Task UpdateItemTypeAsync(ItemTypeDto itemType);

    /// <summary>
    /// Deletes an item type by ID.
    /// </summary>
    Task DeleteItemTypeAsync(int id);
}
