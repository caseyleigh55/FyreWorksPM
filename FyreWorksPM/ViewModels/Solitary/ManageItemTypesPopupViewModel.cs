using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Services.Item;

namespace FyreWorksPM.ViewModels.Solitary;

/// <summary>
/// ViewModel for managing all ItemTypes, allowing creation, editing, and deletion.
/// Interacts with the item type service and backs a management popup.
/// </summary>
public partial class ManageItemTypesPopupViewModel
{
    #region ============== Fields and Constructor =====================

    private readonly IItemTypeService _itemTypeService;

    /// <summary>
    /// Observable collection of item types bound to the UI.
    /// </summary>
    public ObservableCollection<ItemTypeDto> ItemTypes { get; set; } = new();


    /// <summary>
    /// Constructor that initializes the ViewModel and loads item types.
    /// </summary>
    public ManageItemTypesPopupViewModel(IItemTypeService itemTypeService)
    {
        _itemTypeService = itemTypeService;
        _ = LoadItemTypesAsync();
    }

    #endregion

    #region ============== Data Loading ==============================

    /// <summary>
    /// Loads all item types from the database.
    /// </summary>
    private async Task LoadItemTypesAsync()
    {
        var types = await _itemTypeService.GetAllItemTypesAsync();
        ItemTypes.Clear();

        foreach (var type in types)
            ItemTypes.Add(type);
    }

    #endregion

    #region ============== Commands ===================================

    /// <summary>
    /// Adds a new item type entry with an empty name (user-editable).
    /// </summary>
    [RelayCommand]
    private void AddNewType()
    {
        ItemTypes.Add(new ItemTypeDto
        {
            Id = 0, // optional, since it's new
            Name = string.Empty
        });
    }

    /// <summary>
    /// Saves an item type to the database if its name is not empty.
    /// </summary>
    /// <param name="itemType">The item type to save.</param>
    [RelayCommand]
    private async Task Save(ItemTypeDto itemType)
    {
        if (!string.IsNullOrWhiteSpace(itemType.Name))
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
    private async Task Delete(ItemTypeDto itemType)
    {
        await _itemTypeService.DeleteItemTypeAsync(itemType.Id);
        ItemTypes.Remove(itemType);
    }

    #endregion
}
