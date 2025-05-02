using FyreWorksPM.Pages.Creation;
using FyreWorksPM.Services.Client;
using FyreWorksPM.ViewModels.Creation;

namespace FyreWorksPM.Pages.Creation;

/// <summary>
/// Code-behind for CreateBidPage.
/// Responsible for initializing the page and wiring up events like adding a new client.
/// </summary>
public partial class CreateBidPage : ContentPage
{
    private readonly CreateBidViewModel _viewModel;
    private readonly CreateClientPage _createClientPage;

    public CreateBidPage(CreateBidViewModel vm, CreateClientPage createClientPage)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
        _createClientPage = createClientPage;

        // 🎯 Hook the "RequestAddNewClient" event so the ViewModel can tell the Page to open the New Client popup
        _viewModel.RequestAddNewClient = OpenCreateClientPopupAsync;
    }

    /// <summary>
    /// Opens the CreateClientPage as a modal and hooks the callback for when a new client is added.
    /// </summary>
    private async Task OpenCreateClientPopupAsync()
    {
        // Get CreateClientPage fully wired via DI
        var createClientPage = _createClientPage;

        if (createClientPage?.BindingContext is CreateClientViewModel clientVm)
        {
            clientVm.ClientAddedCallback = async (newClient) =>
            {
                // 1️⃣ Reload clients
                await _viewModel.LoadClientsAsync();

                // 2️⃣ Wait a small moment to ensure UI binding has updated
                await Task.Delay(100); // 100ms is plenty for most UIs

                // 3️⃣ Safely update selected client
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var addedClient = _viewModel.Clients.FirstOrDefault(c => c.Id == newClient.Id);
                    if (addedClient != null)
                    {
                        _viewModel.SelectedClient = addedClient;                        
                    }
                });
            };
        }

        // Open the CreateClient page as modal
        await Shell.Current.Navigation.PushModalAsync(createClientPage);
    }
}
