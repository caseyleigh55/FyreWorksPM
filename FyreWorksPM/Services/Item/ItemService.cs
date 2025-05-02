using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FyreWorksPM.Services.Item;

/// <summary>
/// Service for performing CRUD operations on items.
/// </summary>
public class ItemService : IItemService
{
    private readonly ApplicationDbContext _db;

    /// <summary>
    /// Injects the application's database context.
    /// </summary>
    public ItemService(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves all items from the database, including their item type.
    /// </summary>
    public async Task<List<ItemModel>> GetAllItemsAsync()
    {
        return await _db.Items
            .Include(i => i.ItemType)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a single item by ID.
    /// </summary>
    public async Task<ItemModel?> GetItemByIdAsync(int id)
    {
        return await _db.Items
            .Include(i => i.ItemType)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    /// <summary>
    /// Adds a new item to the database.
    /// </summary>
    public async Task AddItemAsync(ItemModel item)
    {
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing item.
    /// </summary>
    public async Task UpdateItemAsync(ItemModel item)
    {
        _db.Items.Update(item);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an item by ID.
    /// </summary>
    public async Task DeleteItemAsync(int id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item != null)
        {
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
