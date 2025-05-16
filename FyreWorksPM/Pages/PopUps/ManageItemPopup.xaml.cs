using FyreWorksPM.ViewModels.Solitary;
using FyreWorksPM.Services.Item;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Services;

namespace FyreWorksPM.Pages.PopUps;

/// <summary>
/// Code-behind for the item edit popup.
/// Hosts the editing UI and handles cancel/navigation behavior.
/// </summary>
public partial class ManageItemPopup : ContentPage, IHideFlyout
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
    private bool isClosingInternally = false;

    public void MarkClosingInternally()
    {
        isClosingInternally = true;
    }


    protected override async void OnDisappearing()
        {
            base.OnDisappearing();
        
        // Safety check to avoid navigation loops
        if (!isClosingInternally)
        {
            await Shell.Current.Navigation.PopAsync();
        }
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);

        if (!isClosingInternally)
        {
            // Use fallback navigation if Shell.Current is null
            var nav = Shell.Current?.Navigation ?? Application.Current.MainPage.Navigation;

            // Dispatch to let UI cycle settle
            _ = MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (nav.NavigationStack.Count > 1)
                {
                    await nav.PopAsync();
                }
            });
        }
    }







    /// <summary>
    /// Handles the cancel button click by closing the popup view.
    /// </summary>
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
