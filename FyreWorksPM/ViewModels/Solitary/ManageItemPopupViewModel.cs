using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;

namespace FyreWorksPM.ViewModels.Solitary;

/// <summary>
/// ViewModel for the item editing popup.
/// Handles loading editable values, saving changes, and refreshing the main view.
/// </summary>
public partial class ManageItemPopupViewModel : ObservableObject
{
    #region Observable Properties

    /// <summary>
    /// Editable name of the item.
    /// </summary>
    [ObservableProperty] private string name;

    /// <summary>
    /// Editable description of the item.
    /// </summary>
    [ObservableProperty] private string description;

    /// <summary>
    /// Editable item type name (can create a new one on save).
    /// </summary>
    [ObservableProperty] private string itemTypeName;

    #endregion

    private readonly ItemModel _item;
    private readonly Func<Task> _onSaved;

    /// <summary>
    /// Constructor for popup ViewModel. Takes the item being edited and a callback for post-save refresh.
    /// </summary>
    /// <param name="item">The item to be edited.</param>
    /// <param name="onSaved">Callback executed after a successful save (usually triggers a UI refresh).</param>
    public ManageItemPopupViewModel(ItemModel item, Func<Task> onSaved)
    {
        _item = item;
        _onSaved = onSaved;

        name = item.Name;
        description = item.Description;
        itemTypeName = item.ItemType?.Name ?? string.Empty;
    }

    /// <summary>
    /// Saves changes made to the item and its associated type.
    /// Adds new item types if they don't already exist.
    /// Triggers a UI refresh and closes the popup.
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        _item.Name = Name;
        _item.Description = Description;

        using var db = new ApplicationDbContextFactory().CreateDbContext(Array.Empty<string>());

        // Look up or create the ItemType
        var existingType = db.ItemTypes.FirstOrDefault(t => t.Name.ToLower() == ItemTypeName.ToLower());
        if (existingType == null)
        {
            existingType = new ItemTypeModel
            {
                Name = ItemTypeName,
                Items = new List<ItemModel>()
            };

            db.ItemTypes.Add(existingType);
            await db.SaveChangesAsync(); // Save to assign Id
        }

        // Link item to the type
        _item.ItemType = existingType;

        db.Items.Update(_item);
        await db.SaveChangesAsync();

        // Trigger the refresh callback from ItemsViewModel
        if (_onSaved != null)
            await _onSaved.Invoke();

        // Close the popup
        await Shell.Current.Navigation.PopAsync();
    }
}
