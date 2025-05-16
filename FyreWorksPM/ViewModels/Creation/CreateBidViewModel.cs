using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Enums;
using FyreWorksPM.Pages.Creation;
using FyreWorksPM.Services.Bid;
using FyreWorksPM.Services.Client;
using FyreWorksPM.Services.Item;
using FyreWorksPM.Services.Navigation;
using FyreWorksPM.Services.Tasks;
using Microsoft.Maui.ApplicationModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace FyreWorksPM.ViewModels.Creation;

/// <summary>
/// ViewModel for creating and managing bids, tasks, and site info.
/// </summary>
public partial class CreateBidViewModel : ObservableObject
{
    // ========== Services ==========
    private readonly IClientService _clientService;
    private readonly IItemService _itemService;
    private readonly IItemTypeService _itemTypeService;
    private readonly IBidService _bidService;
    private readonly ITaskService _taskService;
    private readonly INavigationServices _navigationService;

    public CreateBidViewModel(
        IBidService bidService,
        IClientService clientService,
        IItemService itemService,
        IItemTypeService itemTypeService,
        ITaskService taskService,
        INavigationServices navigationService)
    {
        _bidService = bidService;
        _clientService = clientService;
        _itemService = itemService;
        _itemTypeService = itemTypeService;
        _taskService = taskService;
        _navigationService = navigationService;

        CreatedDate = DateTime.Today;

        NavigateToCreateTasksCommand = new AsyncRelayCommand(NavigateToCreateTasksAsync);

        AdminTasks.CollectionChanged += (s, e) => HookTaskHandlers(e, RaiseAdminTotalsChanged);
        EngineeringTasks.CollectionChanged += (s, e) => HookTaskHandlers(e, RaiseEngineeringTotalsChanged);

        Task.Run(async () => await InitializeAsync());
        
    }

    // ========== Observables ==========

    [ObservableProperty] private bool isActive = true;
    [ObservableProperty] private string bidNumber = string.Empty;
    [ObservableProperty] private string scopeOfWork = string.Empty;
    [ObservableProperty] private string addressLine1 = string.Empty;
    [ObservableProperty] private string addressLine2 = string.Empty;
    [ObservableProperty] private string city = string.Empty;
    [ObservableProperty] private string state = string.Empty;
    [ObservableProperty] private string zipCode = string.Empty;
    [ObservableProperty] private string parcelNumber = string.Empty;
    [ObservableProperty] private string jurisdiction = string.Empty;
    [ObservableProperty] private double buildingArea;
    [ObservableProperty] private int numberOfStories;
    [ObservableProperty] private string occupancyGroup = string.Empty;
    [ObservableProperty] private int occupantLoad;
    [ObservableProperty] private string constructionType = string.Empty;
    [ObservableProperty] private bool isSprinklered;
    [ObservableProperty] private string jobName = string.Empty;
    [ObservableProperty] private DateTime createdDate;
    [ObservableProperty] private string projectName = string.Empty;
    [ObservableProperty] private decimal materialMarkup;
    [ObservableProperty] private decimal laborSubtotal;
    [ObservableProperty] private decimal laborMarkup;
    [ObservableProperty] private ClientDto? selectedClient;


    [ObservableProperty] private ObservableCollection<SavedTaskDto> taskTemplates;
    [ObservableProperty] private SavedTaskDto selectedTemplateTask;
    [ObservableProperty] private BidTaskModel currentTask;




    public ObservableCollection<BidTaskViewModel> Tasks { get; set; } = new();   


    public ObservableCollection<ClientDto> Clients { get; } = new();
    public ObservableCollection<ItemDto> AvailableItems { get; } = new();
    public ObservableCollection<BidLineItemModel> LineItems { get; } = new();

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

    public decimal MaterialSubtotal => LineItems.Sum(i => i.TotalCost);
    public decimal GrandTotal => MaterialSubtotal * (1 + (MaterialMarkup / 100)) + LaborSubtotal * (1 + (LaborMarkup / 100));

    public ObservableCollection<BidTaskViewModel> AdminTasks { get; } = new();
    [ObservableProperty]
    private ObservableCollection<SavedTaskDto> allAdminTaskNames = new();
    public ObservableCollection<BidTaskViewModel> EngineeringTasks { get; } = new();
    [ObservableProperty]
    private ObservableCollection<SavedTaskDto> allEngineeringTaskNames = new();

    public IAsyncRelayCommand NavigateToCreateTasksCommand { get; }


    public decimal AdminCostTotal => AdminTasks.Sum(t => t.Cost);
    public decimal AdminSaleTotal => AdminTasks.Sum(t => t.Sale);
    public decimal EngineeringCostTotal => EngineeringTasks.Sum(t => t.Cost);
    public decimal EngineeringSaleTotal => EngineeringTasks.Sum(t => t.Sale);
    public decimal AdminEngCostTotal => AdminCostTotal + EngineeringCostTotal;
    public decimal AdminEngSaleTotal => AdminSaleTotal + EngineeringSaleTotal;

    private async Task NavigateToCreateTasksAsync()
    {
        await _navigationService.GoToAsync(nameof(CreateTasksPage));
    }


    [RelayCommand]
    private async Task OpenTaskManagerAsync()
    {
        await _navigationService.PushPageAsync<CreateTasksPage>();
    }


    private void HookTaskHandlers(System.Collections.Specialized.NotifyCollectionChangedEventArgs e, Action raiseTotals)
    {
        if (e.NewItems != null)
        {
            foreach (BidTaskViewModel item in e.NewItems)
                item.PropertyChanged += (_, __) => raiseTotals();
        }
        raiseTotals();
    }

    


    private void RaiseAdminTotalsChanged()
    {
        OnPropertyChanged(nameof(AdminCostTotal));
        OnPropertyChanged(nameof(AdminSaleTotal));
        OnPropertyChanged(nameof(AdminEngCostTotal));
        OnPropertyChanged(nameof(AdminEngSaleTotal));
    }

    private void RaiseEngineeringTotalsChanged()
    {
        OnPropertyChanged(nameof(EngineeringCostTotal));
        OnPropertyChanged(nameof(EngineeringSaleTotal));
        OnPropertyChanged(nameof(AdminEngCostTotal));
        OnPropertyChanged(nameof(AdminEngSaleTotal));
    }

    [RelayCommand] private void AddAdminTask() => AddTask(AdminTasks, RaiseAdminTotalsChanged);
    [RelayCommand] private void RemoveAdminTask(BidTaskViewModel task) => RemoveTask(AdminTasks, task, RaiseAdminTotalsChanged);

    [RelayCommand] private void AddEngineeringTask() => AddTask(EngineeringTasks, RaiseEngineeringTotalsChanged);
    [RelayCommand] private void RemoveEngineeringTask(BidTaskViewModel task) => RemoveTask(EngineeringTasks, task, RaiseEngineeringTotalsChanged);

    private void AddTask(ObservableCollection<BidTaskViewModel> tasks, Action raise)
    {
        var task = new BidTaskViewModel();
        task.PropertyChanged += (_, __) => raise();
        tasks.Add(task);
        raise();
    }

    private void RemoveTask(ObservableCollection<BidTaskViewModel> tasks, BidTaskViewModel task, Action raise)
    {
        if (tasks.Contains(task))
        {
            task.PropertyChanged -= (_, __) => raise();
            tasks.Remove(task);
            raise();
        }
    }

    [RelayCommand]
    private void SaveTasks()
    {
        // Optional: log or validate tasks here
        var validTasks = Tasks
            .Where(t =>
                !string.IsNullOrWhiteSpace(t.Name) &&
                t.Cost >= 0 &&
                t.Sale >= 0)
            .ToList();

        if (validTasks.Count == 0)
        {
            // You can show a message here if you want to warn the user
            Debug.WriteLine("[SAVE TASKS] No valid tasks to save.");
            return;
        }

        Debug.WriteLine($"[SAVE TASKS] Preparing {validTasks.Count} tasks for save...");

        foreach (var task in validTasks)
        {
            Debug.WriteLine($"    • {task.Name}, Cost: {task.Cost}, Sale: {task.Sale}, Type: {task.Type}");
            // You can also trim name or do deeper validation here
        }

        // The Tasks list is already in place; this method just confirms the state
        // If you want to replace the list with validated tasks only:
        Tasks.Clear();
        foreach (var task in validTasks)
            Tasks.Add(task);
    }



    [RelayCommand]
    public async Task OpenItemLibraryAsync()
    {
        var vm = new CreateItemsViewModel(_itemService, _itemTypeService,_navigationService);
        await Shell.Current.Navigation.PushAsync(new CreateItemsPage(vm, item => SelectedItemFromLibrary = item));
    }

    [RelayCommand]
    public async Task AddNewClientAsync()
    {
        if (RequestAddNewClient != null)
            await RequestAddNewClient.Invoke();
    }

    [RelayCommand]
    public async Task SaveBidAsync()
    {
        SaveTasks();
        if (string.IsNullOrWhiteSpace(BidNumber) || string.IsNullOrWhiteSpace(ProjectName) || SelectedClient == null)
        {
            await Shell.Current.DisplayAlert("Missing Info", "Please fill in all required fields.", "OK");
            return;
        }
        foreach (var task in Tasks)
        {
            Debug.WriteLine($"[SAVE DEBUG] Task: {task.Name}, Cost: {task.Cost}, Sale: {task.Sale}, Type: {task.Type}");
        }
        var newBid = new CreateBidDto
        {
            BidNumber = BidNumber,
            ProjectName = ProjectName,
            ClientId = SelectedClient.Id,
            CreatedDate = CreatedDate,
            IsActive = IsActive,
            SiteInfo = new SiteInfoDto
            {
                ScopeOfWork = ScopeOfWork,
                AddressLine1 = AddressLine1,
                AddressLine2 = AddressLine2,
                City = City,
                State = State,
                ZipCode = ZipCode,
                ParcelNumber = ParcelNumber,
                Jurisdiction = Jurisdiction,
                BuildingArea = BuildingArea,
                NumberOfStories = NumberOfStories,
                OccupancyGroup = OccupancyGroup,
                OccupantLoad = OccupantLoad,
                ConstructionType = ConstructionType,
                IsSprinklered = IsSprinklered
            },

            Tasks = AdminTasks
                    .Concat(EngineeringTasks)
                    .Select(t => new CreateBidTaskDto
                    {
                        TaskModelId = t.TaskModelId,
                        TaskName = t.Name,
                        Cost = t.Cost,
                        Sale = t.Sale
                    })
                    .ToList()
        };

        try
        {
            await _bidService.CreateBidAsync(newBid);
            await Shell.Current.DisplayAlert("Success", "Bid saved successfully.", "OK");
            await Shell.Current.Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Could not save bid: {ex.Message}", "OK");
        }
    }

    public async Task OnAppearingAsync() => await InitializeAsync();

    private async Task InitializeAsync()
    {
        await ResetFormAsync();
        BidNumber = await _bidService.GetNextBidNumberAsync();
        await Task.WhenAll(LoadClientsAsync(), LoadItemsAsync(), LoadTaskTemplatesAsync());
    }

    public async Task LoadTaskTemplatesAsync()
    {
        var admin = await _taskService.GetTemplatesByTypeAsync(TaskType.Admin);
        allAdminTaskNames = new ObservableCollection<SavedTaskDto>(admin);


        var eng = await _taskService.GetTemplatesByTypeAsync(TaskType.Engineering);
        allEngineeringTaskNames = new ObservableCollection<SavedTaskDto>(eng);

    }

    public Task ResetFormAsync()
    {
        ProjectName = string.Empty;
        SelectedClient = null;
        CreatedDate = DateTime.Today;
        IsActive = true;
        ScopeOfWork = string.Empty;
        AddressLine1 = string.Empty;
        AddressLine2 = string.Empty;
        City = string.Empty;
        State = string.Empty;
        ZipCode = string.Empty;
        ParcelNumber = string.Empty;
        Jurisdiction = string.Empty;
        BuildingArea = 0;
        NumberOfStories = 0;
        OccupancyGroup = string.Empty;
        OccupantLoad = 0;
        ConstructionType = string.Empty;
        IsSprinklered = false;
        return Task.CompletedTask;
    }

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

    public Func<Task>? RequestAddNewClient { get; set; }

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
}