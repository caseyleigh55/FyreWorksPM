using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.Services.Item;
using Microsoft.EntityFrameworkCore;

public class ItemTypeService : IItemTypeService
{
    private readonly ApplicationDbContext _db;

    public ItemTypeService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<string>> GetAllItemTypeNamesAsync()
    {
        return await _db.ItemTypes
            .Select(t => t.Name)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<ItemTypeModel>> GetAllItemTypesAsync()
    {
        return await _db.ItemTypes.ToListAsync();
    }

    public async Task<ItemTypeModel?> GetItemTypeByIdAsync(int id)
    {
        return await _db.ItemTypes.FindAsync(id);
    }

    public async Task AddItemTypeAsync(ItemTypeModel itemType)
    {
        _db.ItemTypes.Add(itemType);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateItemTypeAsync(ItemTypeModel itemType)
    {
        _db.ItemTypes.Update(itemType);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteItemTypeAsync(int id)
    {
        var itemType = await _db.ItemTypes.FindAsync(id);
        if (itemType != null)
        {
            _db.ItemTypes.Remove(itemType);
            await _db.SaveChangesAsync();
        }
    }
}
