using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Pages.Editing;
using FyreWorksPM.ViewModels.Creation;

namespace FyreWorksPM.Pages.Creation;

/// <summary>
/// Page for creating a new client. Uses DI to inject the view model.
/// </summary>
public partial class CreateClientPage : ContentPage
{
    private int _suggestionIndex = -1;
    private readonly Action<ClientDto>? _onClientSelected;
    private readonly CreateClientViewModel _viewModel;

    //===============================================\\
    //================= Constructor =================\\
    //===============================================\\

    /// <summary>
    /// Initializes the ClientsPage with a ViewModel and an optional CLient-selected callback.
    /// </summary>
    /// <param name="vm">The CreateItemsViewModel for this page.</param>
    /// <param name="onClientSelected">Optional callback to fire when an item is selected from the list.</param>
    public CreateClientPage(CreateClientViewModel vm, Action<ClientDto>? onClientSelected = null)
	{
		InitializeComponent();
		BindingContext = vm;
        _viewModel = vm;
        _onClientSelected = onClientSelected;        

        vm.RequestEditClientPopup = async () =>
        {
            var selectedClient = vm.SelectedClient;
            if (selectedClient == null)
                return;

            var popup = new EditClientPage(
                selectedClient,
                async () =>
                {
                    await vm.LoadClientsAsync();
                    vm.FilterClients();
                },
                vm.ClientService  // Or however you access the item service from the view model
            );

            await Shell.Current.Navigation.PushModalAsync(popup);
        };
    }

    /// <summary>
    /// Internal method triggered when a client is selected.
    /// Fires callback and optionally closes the page.
    /// </summary>
    /// <param name="client">The client selected by the user.</param>
    private async void OnClientSelectedInternal(ClientDto client)
    {
        _onClientSelected?.Invoke(client);
        await Shell.Current.Navigation.PopAsync();
    }

    public void FocusClientName()
    {
        ClientNameEntry.Focus();
    }
    /// <summary>
    /// Handles the cancel button click by closing the popup view.
    /// </summary>
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}