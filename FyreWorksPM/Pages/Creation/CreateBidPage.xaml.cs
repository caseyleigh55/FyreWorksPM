using FyreWorksPM.ViewModels.Creation;
using System.Diagnostics;

namespace FyreWorksPM.Pages.Creation;

/// <summary>
/// Code-behind for CreateBidPage.
/// Responsible for initializing the page and wiring up events like adding a new client.
/// </summary>
[QueryProperty(nameof(SelectedTemplateId), "SelectedTemplateId")]
[QueryProperty(nameof(FromEdit), "FromEdit")]
public partial class CreateBidPage : ContentPage
{
    //public string SelectedTemplateId { get; set; }
    private string _selectedTemplateId;
    public string? SelectedTemplateId
    {
        get => _selectedTemplateId;
        set
        {
            _selectedTemplateId = value;
            System.Diagnostics.Debug.WriteLine($"SelectedTemplateId received: {value}");
        }
    }

    public string FromEdit { get; set; }

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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is IViewModelLifecycle vm)
        {
            await vm.OnAppearingAsync();
        }

        if (BindingContext is CreateBidViewModel bidVm)
        {
            // ⬇️ Check if we’re coming from the Edit page
            if (FromEdit == "true" && Guid.TryParse(SelectedTemplateId, out var selectedId))
            {
                System.Diagnostics.Debug.WriteLine($"FromEdit: {FromEdit}, TemplateId: {SelectedTemplateId}");

                await bidVm.LoadTemplateByIdAsync(selectedId);
            }
            else
            {
                await bidVm.LoadTaskTemplatesAsync();
                await bidVm.LoadItemsAsync();
            }
        }
    }

    



    private void OnComponentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.CurrentSelection.FirstOrDefault() as BidComponentLineItemViewModel;
        Debug.WriteLine($"🔥 SelectionChanged event fired! {selected?.ItemName ?? "null"}");
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
