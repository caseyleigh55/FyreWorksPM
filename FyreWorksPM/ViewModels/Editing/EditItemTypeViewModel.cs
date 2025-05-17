using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Services.Item;

namespace FyreWorksPM.ViewModels.Solitary;

/// <summary>
/// ViewModel for managing all ItemTypes, allowing creation, editing, and deletion.
/// Interacts with the item type service and backs a management popup.
/// </summary>
public partial class EditItemTypeViewModel : ObservableObject
{
    private readonly IItemTypeService _itemTypeService;

    [ObservableProperty]
    private ObservableCollection<ItemTypeDto> itemTypes = new();

    public EditItemTypeViewModel(IItemTypeService itemTypeService)
    {
        _itemTypeService = itemTypeService;
        _ = LoadItemTypesAsync();
    }

    /// <summary>
    /// Loads all item types from the database and refreshes the observable collection.
    /// </summary>
    private async Task LoadItemTypesAsync()
    {
        var types = await _itemTypeService.GetAllItemTypesAsync();
        ItemTypes.Clear();

        foreach (var type in types)
            ItemTypes.Add(type);
    }

    /// <summary>
    /// Adds a new item type entry with an empty name (user-editable).
    /// </summary>
    [RelayCommand]
    private void AddNewType()
    {
        ItemTypes.Add(new ItemTypeDto
        {
            ItemTypeDtoId = 0,
            ItemTypeDtoName = string.Empty
        });
    }

    /// <summary>
    /// Saves an item type to the database if its name is not empty.
    /// </summary>
    /// <param name="itemType">The item type to save.</param>
    [RelayCommand]
    private async Task SaveAsync(ItemTypeDto itemType)
    {
        if (!string.IsNullOrWhiteSpace(itemType.ItemTypeDtoName))
        {
            await _itemTypeService.UpdateItemTypeAsync(itemType);
            await LoadItemTypesAsync();
        }
    }

    /// <summary>
    /// Deletes the specified item type by ID.
    /// </summary>
    /// <param name="itemType">The item type to delete.</param>
    [RelayCommand]
    private async Task DeleteAsync(ItemTypeDto itemType)
    {
        await _itemTypeService.DeleteItemTypeAsync(itemType.ItemTypeDtoId);
        ItemTypes.Remove(itemType);
    }
}