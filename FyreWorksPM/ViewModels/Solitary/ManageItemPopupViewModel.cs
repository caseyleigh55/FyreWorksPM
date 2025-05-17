using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Pages.Editing;
using FyreWorksPM.Pages.PopUps;
using FyreWorksPM.Services.Item;

namespace FyreWorksPM.ViewModels.Solitary;

/// <summary>
/// ViewModel for the item editing popup.
/// Handles loading editable values, saving changes, and refreshing the main view.
/// </summary>
public partial class EditItemPageViewModel : ObservableObject
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

    private readonly ItemDto _item;
    private readonly Func<Task> _onSaved;
    private readonly IItemService _itemService;

    /// <summary>
    /// Constructor for popup ViewModel. Takes the item being edited, a callback, and the injected item service.
    /// </summary>
    public EditItemPageViewModel(ItemDto item, Func<Task> onSaved, IItemService itemService)
    {
        _item = item;
        _onSaved = onSaved;
        _itemService = itemService;

        name = item.Name;
        description = item.Description;
        itemTypeName = item.ItemTypeName;
    }

    /// <summary>
    /// Saves changes made to the item using the API.
    /// Triggers a UI refresh and closes the popup.
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        var view = Shell.Current.CurrentPage as EditItemPage;
        view?.MarkClosingInternally();
        _item.Name = Name;
        _item.Description = Description;
        _item.ItemTypeName = ItemTypeName;

        // Call the API via IItemService
        var dto = new CreateItemDto
        {
            Name = _item.Name,
            Description = _item.Description,
            ItemTypeName = _item.ItemTypeName
        };

        await _itemService.UpdateItemAsync(_item.Id, dto);

        // Notify parent to refresh the list
        if (_onSaved != null)
            await _onSaved.Invoke();

        // Proceed with save logic
        await Shell.Current.GoToAsync("..");

        //// Close the popup
        //await Shell.Current.Navigation.PopAsync();
    }
}
