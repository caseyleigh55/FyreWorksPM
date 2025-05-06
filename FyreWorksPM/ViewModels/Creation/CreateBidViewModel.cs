using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Pages.Creation;
using FyreWorksPM.Services.Bid;
using FyreWorksPM.Services.Client;
using FyreWorksPM.Services.Item;

namespace FyreWorksPM.ViewModels.Creation;

/// <summary>
/// ViewModel for managing a bid form, including line items, client list, and item library lookup.
/// </summary>
public partial class CreateBidViewModel : ViewModelBase
{
    // ========================================
    // ============== Services ===============
    // ========================================

    private readonly IClientService _clientService;
    private readonly IItemService _itemService;
    private readonly IItemTypeService _itemTypeService;
    private readonly IBidService _bidService;

    // ========================================
    // ============== Properties =============
    // ========================================

    [ObservableProperty]
    private bool isActive = true; // Default to true
    public string JobName { get => Get<string>(); set => Set(value); }
    public DateTime CreatedDate { get => Get<DateTime>(); set => Set(value); }  

    /// <summary>
    /// List of clients for the client picker or autocomplete.
    /// </summary>
    public ObservableCollection<ClientDto> Clients { get; set; } = new();

    public ClientDto? SelectedClient
    {
        get => Get<ClientDto>();
        set => Set(value);
    }

    public string BidNumber
    {
        get => Get<string>();
        set => Set(value);
    }

    public string ProjectName
    {
        get => Get<string>();
        set => Set(value);
    }



    public ObservableCollection<ItemDto> AvailableItems { get; set; } = new();
    public ObservableCollection<BidLineItemModel> LineItems { get; set; } = new();

    private ItemDto _selectedItemFromLibrary;
    public ItemDto SelectedItemFromLibrary
    {
        get => _selectedItemFromLibrary;
        set
        {
            _selectedItemFromLibrary = value;
            if (value != null)
            {
                LineItems.Add(new BidLineItemModel
                {
                    ItemName = value.Name,
                    UnitCost = 0,
                    Quantity = 1,
                    MarkupPercent = 0
                });

                _selectedItemFromLibrary = null;
                OnPropertyChanged(nameof(LineItems));
                OnPropertyChanged(nameof(MaterialSubtotal));
                OnPropertyChanged(nameof(GrandTotal));
            }
        }
    }

    // ========================================
    // ============== Totals =================
    // ========================================

    public decimal MaterialMarkup { get => Get<decimal>(); set { if (Set(value)) OnPropertyChanged(nameof(GrandTotal)); } }
    public decimal LaborSubtotal { get => Get<decimal>(); set { if (Set(value)) OnPropertyChanged(nameof(GrandTotal)); } }
    public decimal LaborMarkup { get => Get<decimal>(); set { if (Set(value)) OnPropertyChanged(nameof(GrandTotal)); } }

    public decimal MaterialSubtotal => LineItems.Sum(i => i.TotalCost);
    public decimal GrandTotal => MaterialSubtotal * (1 + (MaterialMarkup / 100)) + LaborSubtotal * (1 + (LaborMarkup / 100));

    // ========================================
    // ============== Commands ===============
    // ========================================

    public ICommand AddLineItemCommand { get; }
    public ICommand OpenItemLibraryCommand { get; }
    public ICommand AddNewClientCommand { get; }
    public IRelayCommand SaveBidCommand { get; }


    /// <summary>
    /// Event raised to open the CreateClientPage popup.
    /// </summary>
    public Func<Task> RequestAddNewClient { get; set; }

    // ========================================
    // ============== Constructor ============
    // ========================================

    public CreateBidViewModel(
        IBidService bidService,
        IClientService clientService,
        IItemService itemService,
        IItemTypeService itemTypeService)
    {
        _bidService = bidService;
        _clientService = clientService;
        _itemService = itemService;
        _itemTypeService = itemTypeService;

        CreatedDate = DateTime.Today;

        OpenItemLibraryCommand = new RelayCommand(async () => await OpenItemLibraryAsync());
        AddLineItemCommand = new RelayCommand(AddLineItem);
        AddNewClientCommand = new RelayCommand(async () => await OnRequestAddNewClient());
        SaveBidCommand = new RelayCommand(async () => await SaveBidAsync());
        
        // Call async init safely
        Task.Run(async () => await InitializeAsync());
    }

    private async Task InitializeAsync()
    {
        var nextBidNumber = await _bidService.GetNextBidNumberAsync();
        BidNumber = nextBidNumber;
        await Task.WhenAll(LoadClientsAsync(), LoadItemsAsync());
    }

    // ========================================
    // ============== Data Loading ===========
    // ========================================

    /// <summary>
    /// Loads the list of clients for the picker/autocomplete.
    /// </summary>
    public async Task LoadClientsAsync()
    {
        var clients = await _clientService.GetAllClientsAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Clients.Clear();
            foreach (var client in clients)
                Clients.Add(client);
        });
    }

    /// <summary>
    /// Loads the list of available items.
    /// </summary>
    private async Task LoadItemsAsync()
    {
        var items = await _itemService.GetAllItemsAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            AvailableItems.Clear();
            foreach (var item in items)
                AvailableItems.Add(item);
        });
    }

    // ========================================
    // ============== Logic Helpers ==========
    // ========================================

    /// <summary>
    /// Adds a new blank line item to the bid.
    /// </summary>
    private void AddLineItem()
    {
        LineItems.Add(new BidLineItemModel());
        OnPropertyChanged(nameof(MaterialSubtotal));
        OnPropertyChanged(nameof(GrandTotal));
    }

    /// <summary>
    /// Opens the item library page to select an item.
    /// </summary>
    private async Task OpenItemLibraryAsync()
    {
        var itemService = App.Services.GetRequiredService<IItemService>();
        var itemTypeService = App.Services.GetRequiredService<IItemTypeService>();
        var vm = new CreateItemsViewModel(itemService, itemTypeService);

        await Shell.Current.Navigation.PushAsync(new CreateItemsPage(vm, item =>
        {
            SelectedItemFromLibrary = item;
        }));
    }

    /// <summary>
    /// Opens the popup for creating a new client.
    /// </summary>
    private async Task OnRequestAddNewClient()
    {
        if (RequestAddNewClient != null)
            await RequestAddNewClient.Invoke();
    }

    /// <summary>
    /// Called when a new client is created.
    /// Refreshes the client list and selects the new one.
    /// </summary>
    public async Task OnClientAddedAsync(ClientDto newClient)
    {
        await LoadClientsAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            var matchingClient = Clients.FirstOrDefault(c => c.Id == newClient.Id);
            if (matchingClient != null)
            {
                SelectedClient = matchingClient;
                OnPropertyChanged(nameof(SelectedClient));
            }
        });
    }

    private async Task SaveBidAsync()
    {
        
        if (string.IsNullOrWhiteSpace(BidNumber) || string.IsNullOrWhiteSpace(ProjectName) || SelectedClient == null)
        {

            //await Shell.Current.DisplayAlert("Missing Info", "Please fill in all required fields.", "OK");
            await Shell.Current.DisplayAlert(
    "Missing Info",
    $"Fields:\nBidNumber: '{BidNumber}'\nProjectName: '{ProjectName}'\nClient: '{SelectedClient?.Name}'",
    "OK");


            return;
        }

        var newBid = new CreateBidDto
        {
            BidNumber = BidNumber,
            ProjectName = ProjectName,
            ClientId = SelectedClient.Id,
            CreatedDate = CreatedDate,
            IsActive = this.IsActive // ← Add this line
        };

        try
        {
            await _bidService.CreateBidAsync(newBid);
            await Shell.Current.DisplayAlert("Success", "Bid saved successfully.", "OK");
            await Shell.Current.Navigation.PopAsync(); // Or whatever your page closing method is
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Could not save bid: {ex.Message}", "OK");
        }
    }

}
