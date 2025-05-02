using FyreWorksPM.ViewModels.Solitary;

namespace FyreWorksPM.Pages.PopUps;

/// <summary>
/// Code-behind for the item edit popup.
/// Hosts the editing UI and handles cancel/navigation behavior.
/// </summary>
public partial class ManageItemPopup : ContentPage
{
    /// <summary>
    /// Initializes the popup and sets its binding context.
    /// </summary>
    /// <param name="viewModel">The ViewModel used to edit the selected item.</param>
    public ManageItemPopup(ManageItemPopupViewModel viewModel)
    {
        InitializeComponent();

        // Directly bind the passed ViewModel to this popup's context
        BindingContext = viewModel;

        // If you wanted to prevent edits from auto-updating the original object
        // you could bind to a copy of the ViewModel here instead.
    }

    /// <summary>
    /// Handles the cancel button click by closing the popup view.
    /// </summary>
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }
}
