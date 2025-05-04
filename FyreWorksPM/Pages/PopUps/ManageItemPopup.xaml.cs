using FyreWorksPM.ViewModels.Solitary;
using FyreWorksPM.Services.Item;
using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Pages.PopUps;

/// <summary>
/// Code-behind for the item edit popup.
/// Hosts the editing UI and handles cancel/navigation behavior.
/// </summary>
public partial class ManageItemPopup : ContentPage
{
    /// <summary>
    /// Initializes the popup with the item to edit and post-save callback.
    /// Resolves the item service via DI.
    /// </summary>
    public ManageItemPopup(ItemDto item, Func<Task> onSaved, IItemService service)
    {
        InitializeComponent();        

        // Set up the ViewModel
        BindingContext = new ManageItemPopupViewModel(item, onSaved, service);
    }

    /// <summary>
    /// Handles the cancel button click by closing the popup view.
    /// </summary>
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }
}
