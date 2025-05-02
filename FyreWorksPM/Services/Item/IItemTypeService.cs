using FyreWorksPM.DataAccess.Data.Models;

namespace FyreWorksPM.Services.Item
{
    public interface IItemTypeService
    {
        Task<List<ItemTypeModel>> GetAllItemTypesAsync();
        Task<List<string>> GetAllItemTypeNamesAsync();

        Task<ItemTypeModel?> GetItemTypeByIdAsync(int id);
        Task AddItemTypeAsync(ItemTypeModel itemType);
        Task UpdateItemTypeAsync(ItemTypeModel itemType);
        Task DeleteItemTypeAsync(int id);
    }
}