using FyreWorksPM.DataAccess.Data.Models;

namespace FyreWorksPM.Services.Item;

/// <summary>
/// Interface for item-related database operations.
/// </summary>
public interface IItemService
{
    Task<List<ItemModel>> GetAllItemsAsync();
    Task<ItemModel?> GetItemByIdAsync(int id);
    Task AddItemAsync(ItemModel item);
    Task UpdateItemAsync(ItemModel item);
    Task DeleteItemAsync(int id);
}