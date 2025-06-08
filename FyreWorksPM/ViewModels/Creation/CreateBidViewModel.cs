using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Enums;
using FyreWorksPM.DataAccess.Models;
using FyreWorksPM.Pages.Creation;
using FyreWorksPM.Services.Bid;
using FyreWorksPM.Services.Client;
using FyreWorksPM.Services.Item;
using FyreWorksPM.Services.Labor;
using FyreWorksPM.Services.Navigation;
using FyreWorksPM.Services.Tasks;
using FyreWorksPM.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Input;


namespace FyreWorksPM.ViewModels.Creation;

/// <summary>
/// ViewModel for creating and managing bids, tasks, and site info.
/// </summary>
public partial class CreateBidViewModel : ObservableObject
{
    #region Services

    private readonly IClientService _clientService;
    private readonly IItemService _itemService;
    private readonly IItemTypeService _itemTypeService;
    private readonly IBidService _bidService;
    private readonly ITaskService _taskService;
    private readonly INavigationServices _navigationService;
    private readonly ILaborTemplateService _laborTemplateService;

    #endregion

    #region Constructor

    public CreateBidViewModel(
        IBidService bidService,
        IClientService clientService,
        IItemService itemService,
        IItemTypeService itemTypeService,
        ITaskService taskService,
        INavigationServices navigationService,
        ILaborTemplateService laborTemplateService)
    {
        _bidService = bidService;
        _clientService = clientService;
        _itemService = itemService;
        _itemTypeService = itemTypeService;
        _taskService = taskService;
        _navigationService = navigationService;
        _laborTemplateService = laborTemplateService;

        CreatedDate = DateTime.Today;

        LaborHourMatrix = new ObservableCollection<InstallLocationHoursViewModel>
{
    new() { LocationName = "Warehouse", NormalHours = 0.5m, LiftHours = 1.0m, PanelHours = 0.0m, PipeHours = 1.0m },
    new() { LocationName = "Hardlid", NormalHours = 0.5m, LiftHours = 1.0m, PanelHours = 0.0m, PipeHours = 1.0m },
    new() { LocationName = "T-Bar", NormalHours = 0.25m, LiftHours = 1.0m, PanelHours = 0.0m, PipeHours = 1.0m },
    new() { LocationName = "Underground", NormalHours = 1.0m, LiftHours = 0.0m, PanelHours = 0.0m, PipeHours = 0.0m },
    new() { LocationName = "Panel Room", NormalHours = 1.0m, LiftHours = 0.0m, PanelHours = 1.0m, PipeHours = 0.0m },
    new() { LocationName = "Demo", NormalHours = 0.25m, LiftHours = 0.75m, PanelHours = 1.0m, PipeHours = 1.0m },
    new() { LocationName = "Trim", NormalHours = 0.25m, LiftHours = 0.5m, PanelHours = 3.0m, PipeHours = 0.0m }
};
        DeviceHourSummaries = new ObservableCollection<DeviceHourSummaryViewModel>
{
    new() { ActivityType = "PreWire" },
    new() { ActivityType = "Trim" },
    new() { ActivityType = "Demo" }
};
       

        NavigateToCreateTasksCommand = new AsyncRelayCommand(NavigateToCreateTasksAsync);
        NavigateToCreateItemsCommand = new AsyncRelayCommand(NavigateToCreateItemAsync);

        AdminTasks.CollectionChanged += (s, e) => HookTaskHandlers(e, RaiseAdminTotalsChanged);
        EngineeringTasks.CollectionChanged += (s, e) => HookTaskHandlers(e, RaiseEngineeringTotalsChanged);

        PrewireDeviceHours.CollectionChanged += (_, __) => RaiseComponentTotalsChanged();

        //ComponentLineItems.CollectionChanged += (s, e) => HookTaskHandlers(e, RaiseComponentTotalsChanged);

        MaterialMarkup = 40;
        

        ViewModelHookHelper.AttachCollectionHandlers(ComponentLineItems, (_, __) => RaiseComponentTotalsChanged(),RaiseComponentTotalsChanged);

        Task.Run(async () => await InitializeAsync());
    }

    #endregion

    #region Observable Properties

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
    [ObservableProperty] private decimal laborSubtotal;
    [ObservableProperty] private decimal laborMarkup;
    [ObservableProperty] private ClientDto? selectedClient;

    [ObservableProperty] private ObservableCollection<TaskDto> taskTemplates;
    [ObservableProperty] private TaskDto selectedTemplateTask;
    [ObservableProperty] private BidTaskModel currentTask;

    [ObservableProperty] private decimal materialMarkup;

    private string? _originalLaborTemplateJson;

    public string MaterialExpandButtonText => IsMaterialExpanded ? "△" : "▽";
    public double MaterialListHeight => IsMaterialExpanded ? -1 : 200;
    public ICommand ToggleMaterialExpandCommand => new RelayCommand(() => IsMaterialExpanded = !IsMaterialExpanded);
    private bool _isMaterialExpanded;
    public bool IsMaterialExpanded
    {
        get => _isMaterialExpanded;
        set
        {
            if (SetProperty(ref _isMaterialExpanded, value))
            {
                OnPropertyChanged(nameof(MaterialListHeight));
                OnPropertyChanged(nameof(MaterialExpandButtonText));
            }
        }
    }

    public string WireExpandButtonText => IsWireExpanded ? "△" : "▽";
    public double WireListHeight => IsWireExpanded ? -1 : 200;
    public ICommand ToggleWireExpandCommand => new RelayCommand(() => IsWireExpanded = !IsWireExpanded);
    private bool _isWireExpanded;
    public bool IsWireExpanded
    {
        get => _isWireExpanded;
        set
        {
            if (SetProperty(ref _isWireExpanded, value))
            {
                OnPropertyChanged(nameof(WireListHeight));
                OnPropertyChanged(nameof(WireExpandButtonText));
            }
        }
    }

    public string ComponentExpandButtonText => IsComponentExpanded ? "△" : "▽";
    public double ComponentListHeight => IsComponentExpanded ? -1 : 200;
    public ICommand ToggleComponentExpandCommand => new RelayCommand(() => IsComponentExpanded = !IsComponentExpanded);
    private bool _isComponentExpanded;
    public bool IsComponentExpanded
    {
        get => _isComponentExpanded;
        set
        {
            if (SetProperty(ref _isComponentExpanded, value))
            {
                OnPropertyChanged(nameof(ComponentListHeight));
                OnPropertyChanged(nameof(ComponentExpandButtonText));
            }
        }
    }


    private string _templateName;
    public string TemplateName
    {
        get => _templateName;
        set => SetProperty(ref _templateName, value);
    }

    private bool _isDefaultTemplate;
    public bool IsDefaultTemplate
    {
        get => _isDefaultTemplate;
        set => SetProperty(ref _isDefaultTemplate, value);
    }








    // For SearchableEntryView's selected item
    [ObservableProperty]
    private ItemDto? selectedLibraryItem;

    // For the actual selected row
    [ObservableProperty]
    private BidComponentLineItemViewModel? selectedComponentLineItem;
   

    partial void OnSelectedComponentLineItemChanged(BidComponentLineItemViewModel? value)
    {
        Debug.WriteLine($"📌 SelectedComponentLineItem changed: {value?.ItemName ?? "null"}");
    }


    [ObservableProperty] private ObservableCollection<TaskDto> allAdminTaskNames = new();
    [ObservableProperty] private ObservableCollection<TaskDto> allEngineeringTaskNames = new();

    #endregion

    #region Collections

    public ObservableCollection<BidTaskViewModel> Tasks { get; set; } = new();
    public ObservableCollection<ClientDto> Clients { get; } = new();
    public ObservableCollection<ItemDto> AvailableItems { get; } = new();
    public ObservableCollection<BidWireLineItemModel> LineItems { get; } = new();
    public ObservableCollection<BidTaskViewModel> AdminTasks { get; } = new();
    public ObservableCollection<BidTaskViewModel> EngineeringTasks { get; } = new();

    public ObservableCollection<InstallLocationHoursViewModel> LaborHourMatrix { get; set; }
    public ObservableCollection<DeviceHourSummaryViewModel> DeviceHourSummaries { get; set; }
    public ObservableCollection<LaborSummaryRowViewModel> PrewireDeviceHours { get; set; } = new();
    [ObservableProperty]
    private LaborSummaryRowViewModel trimSummaryRow = new() { Location = "Trim" };
    [ObservableProperty]
    private LaborSummaryRowViewModel demoSummaryRow = new() { Location = "Demo" };



    public ObservableCollection<BidComponentLineItemViewModel> ComponentLineItems { get; } = new();    
    public ObservableCollection<BidWireLineItemViewModel> WireLineItems { get; } = new();
    public ObservableCollection<BidMaterialLineItemViewModel> MaterialLineItems { get; } = new();



    #endregion

    #region Static Option Lists

    public ObservableCollection<string> YesNoOptions { get; } = new() { "Yes", "No" };
    public ObservableCollection<string> InstallTypeOptions { get; } = new() { "Normal", "Lift", "Panel", "Pipe" };
    public ObservableCollection<string> InstallLocationOptions { get; } = new() { "Warehouse", "Hardlid", "T-Bar", "Underground", "Panel Room", "Demo"};

    #endregion

    #region Derived Properties

    public string SelectedSprinklerOption
    {
        get => IsSprinklered ? "Yes" : "No";
        set => IsSprinklered = value == "Yes";
    }


   

    public decimal AdminCostTotal => AdminTasks.Sum(t => t.Cost);
    public decimal AdminSaleTotal => AdminTasks.Sum(t => t.Sale);
    public decimal EngineeringCostTotal => EngineeringTasks.Sum(t => t.Cost);
    public decimal EngineeringSaleTotal => EngineeringTasks.Sum(t => t.Sale);
    public decimal AdminEngCostTotal => AdminCostTotal + EngineeringCostTotal;
    public decimal AdminEngSaleTotal => AdminSaleTotal + EngineeringSaleTotal;

    public decimal CalculatedCostTotal => TotalLaborCost + AdminEngCostTotal + MaterialLineItemsCostTotal + WireLineItemsCostTotal + PanelLineItemsCostTotal;
    public decimal CalculatedSaleTotal => TotalLaborSale + AdminEngSaleTotal + MaterialLineItemsSaleTotal + WireLineItemsSaleTotal + PanelLineItemsSaleTotal;
    public decimal CalculatedProfit => CalculatedSaleTotal - CalculatedCostTotal;
    public decimal CalculatedMargin => CalculatedCostTotal == 0 ? 0 : (CalculatedProfit / CalculatedCostTotal);
    
    private bool _userAdjustedSaleManually = false;

    private decimal _adjustedSaleTotal;
    public decimal AdjustedSaleTotal
    {
        get => _adjustedSaleTotal;
        set
        {
            if (SetProperty(ref _adjustedSaleTotal, value))
            {
                _userAdjustedSaleManually = true;
                OnPropertyChanged(nameof(AdjustedProfit));
                OnPropertyChanged(nameof(AdjustedMargin));
                
            }
        }
    }
    public decimal AdjustedProfit => AdjustedSaleTotal - CalculatedCostTotal;
    public decimal AdjustedMargin => CalculatedCostTotal == 0 ? 0 : (AdjustedProfit / CalculatedCostTotal);

    private void RaiseCostSaleTotalsChanged()
    {
        OnPropertyChanged(nameof(CalculatedCostTotal));
        OnPropertyChanged(nameof(CalculatedSaleTotal));
        OnPropertyChanged(nameof(CalculatedProfit));
        OnPropertyChanged(nameof(CalculatedMargin));
        OnPropertyChanged(nameof(AdjustedProfit));
        OnPropertyChanged(nameof(AdjustedMargin));

        if (!_userAdjustedSaleManually)
        {
            _adjustedSaleTotal = CalculatedSaleTotal;
            // Even if the value is the same, notify dependents manually
            OnPropertyChanged(nameof(AdjustedSaleTotal));
            OnPropertyChanged(nameof(AdjustedProfit));
            OnPropertyChanged(nameof(AdjustedMargin));
            
        }

    }
    private void RaiseLaborHourTotalsChanged()
    {
        OnPropertyChanged(nameof(JourneymanRegularHoursTotal));
        OnPropertyChanged(nameof(JourneymanOvernightHoursTotal));
        OnPropertyChanged(nameof(ApprenticeRegularHoursTotal));
        OnPropertyChanged(nameof(ApprenticeOvernightHoursTotal));

        OnPropertyChanged(nameof(JourneymanCostTotal));
        OnPropertyChanged(nameof(JourneymanSaleTotal));
        OnPropertyChanged(nameof(ApprenticeCostTotal));
        OnPropertyChanged(nameof(ApprenticeSaleTotal));

        OnPropertyChanged(nameof(TotalRegularHours));
        OnPropertyChanged(nameof(TotalOvernightHours));
        OnPropertyChanged(nameof(TotalLaborCost));
        OnPropertyChanged(nameof(TotalLaborSale));
        RaiseCostSaleTotalsChanged();
    }

    // Regular Hours
    private decimal _demoJourneymanHours;
    public decimal DemoJourneymanHours
    {
        get => _demoJourneymanHours;
        set
        {
            if (SetProperty(ref _demoJourneymanHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _prewireJourneymanHours;
    public decimal PrewireJourneymanHours
    {
        get => _prewireJourneymanHours;
        set
        {
            if (SetProperty(ref _prewireJourneymanHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _trimJourneymanHours;
    public decimal TrimJourneymanHours
    {
        get => _trimJourneymanHours;
        set
        {
            if (SetProperty(ref _trimJourneymanHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _testJourneymanHours;
    public decimal TestJourneymanHours
    {
        get => _testJourneymanHours;
        set
        {
            if (SetProperty(ref _testJourneymanHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _demoApprenticeHours;
    public decimal DemoApprenticeHours
    {
        get => _demoApprenticeHours;
        set
        {
            if (SetProperty(ref _demoApprenticeHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _prewireApprenticeHours;
    public decimal PrewireApprenticeHours
    {
        get => _prewireApprenticeHours;
        set
        {
            if (SetProperty(ref _prewireApprenticeHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _trimApprenticeHours;
    public decimal TrimApprenticeHours
    {
        get => _trimApprenticeHours;
        set
        {
            if (SetProperty(ref _trimApprenticeHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _testApprenticeHours;
    public decimal TestApprenticeHours
    {
        get => _testApprenticeHours;
        set
        {
            if (SetProperty(ref _testApprenticeHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    // Overnight Hours
    private decimal _overnightDemoJourneymanHours;
    public decimal OvernightDemoJourneymanHours
    {
        get => _overnightDemoJourneymanHours;
        set
        {
            if (SetProperty(ref _overnightDemoJourneymanHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _overnightPrewireJourneymanHours;
    public decimal OvernightPrewireJourneymanHours
    {
        get => _overnightPrewireJourneymanHours;
        set
        {
            if (SetProperty(ref _overnightPrewireJourneymanHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _overnightTrimJourneymanHours;
    public decimal OvernightTrimJourneymanHours
    {
        get => _overnightTrimJourneymanHours;
        set
        {
            if (SetProperty(ref _overnightTrimJourneymanHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _overnightTestJourneymanHours;
    public decimal OvernightTestJourneymanHours
    {
        get => _overnightTestJourneymanHours;
        set
        {
            if (SetProperty(ref _overnightTestJourneymanHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _overnightDemoApprenticeHours;
    public decimal OvernightDemoApprenticeHours
    {
        get => _overnightDemoApprenticeHours;
        set
        {
            if (SetProperty(ref _overnightDemoApprenticeHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _overnightPrewireApprenticeHours;
    public decimal OvernightPrewireApprenticeHours
    {
        get => _overnightPrewireApprenticeHours;
        set
        {
            if (SetProperty(ref _overnightPrewireApprenticeHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _overnightTrimApprenticeHours;
    public decimal OvernightTrimApprenticeHours
    {
        get => _overnightTrimApprenticeHours;
        set
        {
            if (SetProperty(ref _overnightTrimApprenticeHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    private decimal _overnightTestApprenticeHours;
    public decimal OvernightTestApprenticeHours
    {
        get => _overnightTestApprenticeHours;
        set
        {
            if (SetProperty(ref _overnightTestApprenticeHours, value))
                RaiseLaborHourTotalsChanged();
        }
    }

    public decimal JourneymanRegularDirectRate { get; set; } = 65;
    public decimal JourneymanRegularBilledRate { get; set; } = 90;
    public decimal JourneymanOvernightDirectRate { get; set; } = 75;
    public decimal JourneymanOvernightBilledRate { get; set; } = 115;
    public decimal ApprenticeRegularDirectRate { get; set; } = 30;
    public decimal ApprenticeRegularBilledRate { get; set; } = 54;
    public decimal ApprenticeOvernightDirectRate { get; set; } = 45;
    public decimal ApprenticeOvernightBilledRate { get; set; } = 83;

    public decimal JourneymanRegularHoursTotal =>
    DemoJourneymanHours + PrewireJourneymanHours + TrimJourneymanHours + TestJourneymanHours;

    public decimal JourneymanOvernightHoursTotal =>
        OvernightDemoJourneymanHours + OvernightPrewireJourneymanHours + OvernightTrimJourneymanHours + OvernightTestJourneymanHours;

    public decimal ApprenticeRegularHoursTotal =>
        DemoApprenticeHours + PrewireApprenticeHours + TrimApprenticeHours + TestApprenticeHours;

    public decimal ApprenticeOvernightHoursTotal =>
        OvernightDemoApprenticeHours + OvernightPrewireApprenticeHours + OvernightTrimApprenticeHours + OvernightTestApprenticeHours;

    public decimal JourneymanCostTotal =>
    (JourneymanRegularHoursTotal * JourneymanRegularDirectRate) +
    (JourneymanOvernightHoursTotal * JourneymanOvernightDirectRate);

    public decimal JourneymanSaleTotal =>
        (JourneymanRegularHoursTotal * JourneymanRegularBilledRate) +
        (JourneymanOvernightHoursTotal * JourneymanOvernightBilledRate);

    public decimal ApprenticeCostTotal =>
        (ApprenticeRegularHoursTotal * ApprenticeRegularDirectRate) +
        (ApprenticeOvernightHoursTotal * ApprenticeOvernightDirectRate);

    public decimal ApprenticeSaleTotal =>
        (ApprenticeRegularHoursTotal * ApprenticeRegularBilledRate) +
        (ApprenticeOvernightHoursTotal * ApprenticeOvernightBilledRate);

    // Combined Hour Totals
    public decimal TotalRegularHours => JourneymanRegularHoursTotal + ApprenticeRegularHoursTotal;
    public decimal TotalOvernightHours => JourneymanOvernightHoursTotal + ApprenticeOvernightHoursTotal;

    // Combined Cost and Sale Totals
    public decimal TotalLaborCost => JourneymanCostTotal + ApprenticeCostTotal;
    public decimal TotalLaborSale => JourneymanSaleTotal + ApprenticeSaleTotal;



    // Backing fields
    private decimal _prewireTotalHours;
    private decimal _demoTotalHours;
    private decimal _trimTotalHours;
    private decimal _totalCombinedHours;

    // Public properties
    public decimal PrewireTotalHours
    {
        get => _prewireTotalHours;
        set => SetProperty(ref _prewireTotalHours, value);
    }

    public decimal DemoTotalHours
    {
        get => _demoTotalHours;
        set => SetProperty(ref _demoTotalHours, value);
    }

    public decimal TrimTotalHours
    {
        get => _trimTotalHours;
        set => SetProperty(ref _trimTotalHours, value);
    }

    public decimal TotalCombinedHours
    {
        get => _totalCombinedHours;
        set => SetProperty(ref _totalCombinedHours, value);
    }



    public decimal PanelLineItemsCostTotal => ComponentLineItems.Sum(t => t.UnitCost * t.Qty);
    public decimal PanelLineItemsSaleTotal => ComponentLineItems.Sum(t => t.UnitSale * t.Qty);

    public decimal WireLineItemsCostTotal => WireLineItems.Sum(i => i.UnitCost * i.Qty);
    public decimal WireLineItemsSaleTotal => WireLineItems.Sum(i => i.UnitSale * i.Qty);

    public decimal MaterialLineItemsCostTotal => MaterialLineItems.Sum(i => i.UnitCost * i.Qty);
    public decimal MaterialLineItemsSaleTotal => MaterialLineItems.Sum(i => i.UnitSale * i.Qty);



    public decimal TotalComponentHours => ComponentLineItems.Sum(item => item.TotalHours);


    public BidLaborConfig LaborOverrides { get; set; } = new();

    #endregion

    #region Commands

    public IAsyncRelayCommand NavigateToCreateTasksCommand { get; }
    public IAsyncRelayCommand NavigateToCreateItemsCommand { get; }



    [RelayCommand] private async Task OpenTaskManagerAsync() => await _navigationService.PushPageAsync<CreateTasksPage>();
    [RelayCommand] private void AddAdminTask() => AddTask(AdminTasks, RaiseAdminTotalsChanged);
    [RelayCommand] private void RemoveAdminTask(BidTaskViewModel task) => RemoveTask(AdminTasks, task, RaiseAdminTotalsChanged);
    [RelayCommand] private void AddEngineeringTask() => AddTask(EngineeringTasks, RaiseEngineeringTotalsChanged);
    [RelayCommand] private void RemoveEngineeringTask(BidTaskViewModel task) => RemoveTask(EngineeringTasks, task, RaiseEngineeringTotalsChanged);
    [RelayCommand] private void SaveTasks() => SaveValidTasks();
    //[RelayCommand] public async Task CreateNewItemAsync() => await CreateNewItem();
    


    [RelayCommand]
    private void SelectComponentItem(BidComponentLineItemViewModel item)
    {
        SelectedComponentLineItem = item;
        Debug.WriteLine($"✅ Selected: {item.Item.ItemName}");
    }


    [RelayCommand] private void RemoveComponentItem(BidComponentLineItemViewModel SelectedComponentLineItem) => RemoveComponentItem(ComponentLineItems, SelectedComponentLineItem, RaiseComponentTotalsChanged);

    
    [RelayCommand] private void AddComponentItem() => AddNewComponent();
    [RelayCommand] private void AddWireItem() => AddNewWireItem();
    [RelayCommand] public async Task AddNewClientAsync() => await AddClient();
    [RelayCommand] public async Task SaveBidAsync() => await SaveBid();

    #endregion
    #region Initialization

    public async Task OnAppearingAsync() => await InitializeAsync();

    private async Task InitializeAsync()
    {
        await ResetFormAsync();
        BidNumber = await _bidService.GetNextBidNumberAsync();
        await Task.WhenAll(LoadClientsAsync(), LoadItemsAsync(), LoadTaskTemplatesAsync(), _laborTemplateService.GetDefaultTemplateAsync());
        var defaultTemplate = await _laborTemplateService.GetDefaultTemplateAsync();
        _originalLaborTemplateJson = JsonSerializer.Serialize(defaultTemplate, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Populate your form fields from the defaultTemplate like usual
        ApplyLaborTemplate(defaultTemplate); // <- you likely already have this

    }

    private void ApplyLaborTemplate(LaborTemplateDto template)
    {
        if (template == null) return;

        // Set Labor Rates
        var journeyman = template.LaborRates.FirstOrDefault(r => r.Role == "Journeyman");
        var apprentice = template.LaborRates.FirstOrDefault(r => r.Role == "Apprentice");

        if (journeyman != null)
        {
            JourneymanRegularDirectRate = journeyman.RegularDirectRate;
            JourneymanRegularBilledRate = journeyman.RegularBilledRate;
            JourneymanOvernightDirectRate = journeyman.OvernightDirectRate;
            JourneymanOvernightBilledRate = journeyman.OvernightBilledRate;
        }

        if (apprentice != null)
        {
            ApprenticeRegularDirectRate = apprentice.RegularDirectRate;
            ApprenticeRegularBilledRate = apprentice.RegularBilledRate;
            ApprenticeOvernightDirectRate = apprentice.OvernightDirectRate;
            ApprenticeOvernightBilledRate = apprentice.OvernightBilledRate;
        }

        // Set Location Hours Matrix
        LaborHourMatrix.Clear();
        foreach (var hour in template.LocationHours)
        {
            LaborHourMatrix.Add(new InstallLocationHoursViewModel
            {
                LocationName = hour.LocationName,
                NormalHours = hour.Normal,
                LiftHours = hour.Lift,
                PanelHours = hour.Panel,
                PipeHours = hour.Pipe
            });
        }
    }


    private LaborTemplateDto BuildCurrentLaborTemplateDto()
    {
        return new LaborTemplateDto
        {
            TemplateName = "UnsavedTemplate",
            IsDefault = false,
            LaborRates = new List<LaborRateDto>
        {
            new LaborRateDto
            {
                Role = "Journeyman",
                RegularDirectRate = JourneymanRegularDirectRate,
                RegularBilledRate = JourneymanRegularBilledRate,
                OvernightDirectRate = JourneymanOvernightDirectRate,
                OvernightBilledRate = JourneymanOvernightBilledRate
            },
            new LaborRateDto
            {
                Role = "Apprentice",
                RegularDirectRate = ApprenticeRegularDirectRate,
                RegularBilledRate = ApprenticeRegularBilledRate,
                OvernightDirectRate = ApprenticeOvernightDirectRate,
                OvernightBilledRate = ApprenticeOvernightBilledRate
            }
        },
            LocationHours = LaborHourMatrix.Select(l => new LocationHourDto
            {
                LocationName = l.LocationName,
                Normal = l.NormalHours,
                Lift = l.LiftHours,
                Panel = l.PanelHours,
                Pipe = l.PipeHours
            }).ToList()
        };
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

    #endregion

    [RelayCommand]
    private async Task SaveTemplateAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(TemplateName))
            {
                await Shell.Current.DisplayAlert("Missing Info", "Please enter a name for the template.", "OK");
                return;
            }

            var template = new CreateLaborTemplateDto
            {               
                TemplateName = TemplateName,
                IsDefault = IsDefaultTemplate,
                LaborRates = new List<LaborRateDto>
            {
                new LaborRateDto
                {
                    Role = "Journeyman",
                    RegularDirectRate = JourneymanRegularDirectRate,
                    RegularBilledRate = JourneymanRegularBilledRate,
                    OvernightDirectRate = JourneymanOvernightDirectRate,
                    OvernightBilledRate = JourneymanOvernightBilledRate
                },
                new LaborRateDto
                {
                    Role = "Apprentice",
                    RegularDirectRate = ApprenticeRegularDirectRate,
                    RegularBilledRate = ApprenticeRegularBilledRate,
                    OvernightDirectRate = ApprenticeOvernightDirectRate,
                    OvernightBilledRate = ApprenticeOvernightBilledRate
                }
            },
                
                LocationHours = LaborHourMatrix.Select(l => new LocationHourDto
                {
                    LocationName = l.LocationName,
                    Normal = l.NormalHours,
                    Lift = l.LiftHours,
                    Panel = l.PanelHours,
                    Pipe = l.PipeHours
                }).ToList()
            };

            await _laborTemplateService.CreateTemplateAsync(template);

            await Shell.Current.DisplayAlert("Success", "Template saved successfully!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Could not save template: {ex.Message}", "OK");
        }
    }


    [RelayCommand]
    private async Task LoadTemplateAsync()
    {
        // Load logic here
        Debug.WriteLine("Template loaded!");
    }



    public decimal GetLaborRate(string location, string installType)
    {
        var match = LaborHourMatrix.FirstOrDefault(x =>
            string.Equals(x.LocationName, location, StringComparison.OrdinalIgnoreCase));

        if (match == null)
            return 0;

        return installType.ToLower() switch
        {
            "normal" => match.NormalHours,
            "lift" => match.LiftHours,
            "panel" => match.PanelHours,
            "pipe" => match.PipeHours,
            _ => 0
        };
    }

    public void RecalculateDemoSummary()
    {
        var matrix = LaborHourMatrix.FirstOrDefault(x =>
            x.LocationName.Trim().Equals("demo", StringComparison.OrdinalIgnoreCase));

        if (matrix == null)
        {
            Debug.WriteLine("[DEMO] Matrix row for 'demo' not found!");
            return;
        }

        DemoSummaryRow.Location = "Demo";
        DemoSummaryRow.NormalCount = 0m;
        DemoSummaryRow.LiftCount = 0m;
        DemoSummaryRow.PanelCount = 0m;
        DemoSummaryRow.PipeCount = 0m;
        DemoSummaryRow.TotalHours = 0.0m;

        foreach (var item in ComponentLineItems.Where(i => i.InstallLocation?.Trim().ToLower() == "demo"))
        {
            switch (item.InstallType?.Trim().ToLower())
            {
                case "normal":
                    DemoSummaryRow.NormalCount += item.Qty;
                    DemoSummaryRow.TotalHours += item.Qty * matrix.NormalHours;
                    break;
                case "lift":
                    DemoSummaryRow.LiftCount += item.Qty;
                    DemoSummaryRow.TotalHours += item.Qty * matrix.LiftHours;
                    break;
                case "panel":
                    DemoSummaryRow.PanelCount += item.Qty;
                    DemoSummaryRow.TotalHours += item.Qty * matrix.PanelHours;
                    break;
                case "pipe":
                    DemoSummaryRow.PipeCount += item.Qty;
                    DemoSummaryRow.TotalHours += item.Qty * matrix.PipeHours;
                    break;
            }

            if (item.Piped)
            {
                DemoSummaryRow.PipeCount += item.Qty;
                DemoSummaryRow.TotalHours += item.Qty * matrix.PipeHours;
            }
        }
    }
    private bool _isUpdatingPrewire;


    public void RecalculatePrewireDeviceHours()
    {
        if (_isUpdatingPrewire) return;

        _isUpdatingPrewire = true;

        try
        {
            PrewireDeviceHours.Clear();
        

        var grouped = ComponentLineItems
            .Where(i => i.InstallLocation?.ToLower() != "demo") // Exclude demo
            .GroupBy(i => i.InstallLocation?.ToLower());

            foreach (var group in grouped)
            {
                var location = group.Key ?? string.Empty;
                var matrix = LaborHourMatrix.FirstOrDefault(x =>
        x.LocationName.Trim().Equals(location?.Trim(), StringComparison.OrdinalIgnoreCase));


                if (matrix == null)
                    continue;

                var row = new LaborSummaryRowViewModel
                {
                    Location = matrix.LocationName,
                };

                foreach (var item in group)
                {
                    switch (item.InstallType.ToLower())
                    {
                        case "normal":
                            row.NormalCount += item.Qty;
                            row.TotalHours += item.Qty * matrix.NormalHours;
                            break;
                        case "lift":
                            row.LiftCount += item.Qty;
                            row.TotalHours += item.Qty * matrix.LiftHours;
                            break;
                        case "panel":
                            row.PanelCount += item.Qty;
                            row.TotalHours += item.Qty * matrix.PanelHours;
                            break;
                        case "pipe":
                            row.PipeCount += item.Qty;
                            row.TotalHours += item.Qty * matrix.PipeHours;
                            break;
                    }
                    if (item.Piped)
                    {
                        row.PipeCount += item.Qty;
                        row.TotalHours += item.Qty * matrix.PipeHours;
                    }
                }

                PrewireDeviceHours.Add(row);
            }

        }
        finally
        {
            _isUpdatingPrewire = false;

        }
    }


    public void RecalculateDeviceHourTotals()
    {
        var trimSummary = DeviceHourSummaries.FirstOrDefault(x => x.ActivityType == "Trim");
        if (trimSummary != null)
        {
            trimSummary.NormalCount = 0.0m;
            trimSummary.LiftCount = 0.0m;
            trimSummary.PanelCount = 0.0m;
            trimSummary.PipeCount = 0.0m;
            trimSummary.TotalHours = 0.0m;

            foreach (var item in ComponentLineItems)
            {
                switch (item.InstallType.ToLower())
                {
                    case "normal":
                        trimSummary.NormalCount += item.Qty;
                        trimSummary.TotalHours += item.Qty * LaborHourMatrix.First(x => x.LocationName.ToLower() == item.InstallLocation.ToLower()).NormalHours;
                        break;
                    case "lift":
                        trimSummary.LiftCount += item.Qty;
                        trimSummary.TotalHours += item.Qty * LaborHourMatrix.First(x => x.LocationName.ToLower() == item.InstallLocation.ToLower()).LiftHours;
                        break;
                    case "panel":
                        trimSummary.PanelCount += item.Qty;
                        trimSummary.TotalHours += item.Qty * LaborHourMatrix.First(x => x.LocationName.ToLower() == item.InstallLocation.ToLower()).PanelHours;
                        break;
                    case "pipe":
                        trimSummary.PipeCount += item.Qty;
                        trimSummary.TotalHours += item.Qty * LaborHourMatrix.First(x => x.LocationName.ToLower() == item.InstallLocation.ToLower()).PipeHours;
                        break;
                }
               
            }
        }

        var demoSummary = DeviceHourSummaries.FirstOrDefault(x => x.ActivityType == "Demo");
        if (demoSummary != null)
        {
            demoSummary.NormalCount = 0.0m;
            demoSummary.LiftCount = 0.0m;
            demoSummary.PanelCount = 0.0m;
            demoSummary.PipeCount = 0.0m;
            demoSummary.TotalHours = 0.0m;

            foreach (var item in ComponentLineItems.Where(i => i.InstallLocation?.ToLower() == "demo"))
            {
                switch (item.InstallType.ToLower())
                {
                    case "normal": demoSummary.NormalCount += item.Qty; break;
                    case "lift": demoSummary.LiftCount += item.Qty; break;
                    case "panel": demoSummary.PanelCount += item.Qty; break;
                    case "pipe": demoSummary.PipeCount += item.Qty; break;
                }

                demoSummary.TotalHours += item.TotalHours;
            }
        }

        var prewireSummary = DeviceHourSummaries.FirstOrDefault(x => x.ActivityType == "PreWire");
        if (prewireSummary != null)
        {
            prewireSummary.NormalCount = 0;
            prewireSummary.LiftCount = 0;
            prewireSummary.PanelCount = 0;
            prewireSummary.PipeCount = 0;
            prewireSummary.TotalHours = 0;

            foreach (var item in ComponentLineItems.Where(i => i.InstallLocation?.ToLower() != "demo"))
            {
                switch (item.InstallType.ToLower())
                {
                    case "normal": prewireSummary.NormalCount += item.Qty; break;
                    case "lift": prewireSummary.LiftCount += item.Qty; break;
                    case "panel": prewireSummary.PanelCount += item.Qty; break;
                    case "pipe": prewireSummary.PipeCount += item.Qty; break;
                }

                prewireSummary.TotalHours += item.TotalHours;
            }
        }


    }

    public void RecalculateTrimSummary()
    {

        var matrix = LaborHourMatrix.FirstOrDefault(x =>
            x.LocationName.Trim().Equals("Trim", StringComparison.OrdinalIgnoreCase));

        if (matrix == null)
        {
            return;
        }

        trimSummaryRow.Location = "Trim";
        trimSummaryRow.NormalCount = 0;
        trimSummaryRow.LiftCount = 0;
        trimSummaryRow.PanelCount = 0;
        trimSummaryRow.PipeCount = 0;
        trimSummaryRow.TotalHours = 0;

        foreach (var item in ComponentLineItems)
        {
            if (item.InstallLocation?.Trim().ToLower() == "demo")
                continue; // 🛑 Skip demo items!
            switch (item.InstallType.ToLower())
            {
                case "normal":
                    trimSummaryRow.NormalCount += item.Qty;
                    trimSummaryRow.TotalHours += item.Qty * matrix.NormalHours;
                    break;
                case "lift":
                    trimSummaryRow.LiftCount += item.Qty;
                    trimSummaryRow.TotalHours += item.Qty * matrix.LiftHours;
                    break;
                case "panel":
                    trimSummaryRow.PanelCount += item.Qty;
                    trimSummaryRow.TotalHours += item.Qty * matrix.PanelHours;
                    break;
                case "pipe":
                    trimSummaryRow.PipeCount += item.Qty;
                    trimSummaryRow.TotalHours += item.Qty * matrix.PipeHours;
                    break;               
            }
            if (item.Piped)
            {
                TrimSummaryRow.PipeCount += item.Qty;
                TrimSummaryRow.TotalHours += item.Qty * matrix.PipeHours;
            }
        }
    }

   




    #region Task Templates

    public async Task LoadTaskTemplatesAsync()
    {
        var admin = await _taskService.GetTemplatesByTypeAsync(TaskType.Admin);
        AllAdminTaskNames = new ObservableCollection<TaskDto>(admin);

        var eng = await _taskService.GetTemplatesByTypeAsync(TaskType.Engineering);
        AllEngineeringTaskNames = new ObservableCollection<TaskDto>(eng);
    }

    #endregion

    #region Task Management

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
        RaiseCostSaleTotalsChanged();
    }

    private void RaiseEngineeringTotalsChanged()
    {
        OnPropertyChanged(nameof(EngineeringCostTotal));
        OnPropertyChanged(nameof(EngineeringSaleTotal));
        OnPropertyChanged(nameof(AdminEngCostTotal));
        OnPropertyChanged(nameof(AdminEngSaleTotal));
        RaiseCostSaleTotalsChanged();
    }

    


    private void RaiseComponentTotalsChanged()
    {
        OnPropertyChanged(nameof(TotalComponentHours));
        OnPropertyChanged(nameof(PanelLineItemsCostTotal));
        OnPropertyChanged(nameof(PanelLineItemsSaleTotal));
        RecalculatePrewireDeviceHours();
        RecalculateTrimSummary();
        RecalculateDemoSummary();
        RaiseCostSaleTotalsChanged();

        PrewireTotalHours = PrewireDeviceHours.Sum(x => x.TotalHours);
        DemoTotalHours = DemoSummaryRow?.TotalHours ?? 0;
        TrimTotalHours = TrimSummaryRow?.TotalHours ?? 0;
        TotalCombinedHours = PrewireTotalHours + DemoTotalHours + TrimTotalHours;


    }

    private void RaiseWireTotalsChanged()
    {
        OnPropertyChanged(nameof(WireLineItemsCostTotal));
        OnPropertyChanged(nameof(WireLineItemsSaleTotal));
        RaiseCostSaleTotalsChanged();
    }

    private void RaiseMaterialTotalsChanged()
    {
        OnPropertyChanged(nameof(MaterialLineItemsCostTotal));
        OnPropertyChanged(nameof(MaterialLineItemsSaleTotal));
        RaiseCostSaleTotalsChanged();
    }


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

    private void RemoveComponentItem(ObservableCollection<BidComponentLineItemViewModel> items, BidComponentLineItemViewModel item, Action raise)
    {
        
        if (items.Contains(item))
        {
            item.PropertyChanged -= (_, __) => raise();
            items.Remove(item);
            raise();
        }
    }

    [RelayCommand]
    private void AddNewWireItem()
    {
        
        var item = new BidWireLineItemModel { ItemName = "Wire", Qty = 1, UnitCost = 0, UnitSale = 0 };
        var vm = new BidWireLineItemViewModel(item, this, RaiseWireTotalsChanged);
        WireLineItems.Add(vm);
        OnPropertyChanged(nameof(WireLineItems));
        RaiseWireTotalsChanged();
    }

    [RelayCommand]
    private void RemoveWireItem(BidWireLineItemViewModel item)
    {
        WireLineItems.Remove(item);
        RaiseWireTotalsChanged();
    }


    [RelayCommand]
    private void AddMaterialItem()
    {
        var item = new BidMaterialLineItemModel { ItemName = "Material", Qty = 1, UnitCost = 0, UnitSale = 0 };
        var vm = new BidMaterialLineItemViewModel(item, this, RaiseMaterialTotalsChanged);
        MaterialLineItems.Add(vm);
        RaiseMaterialTotalsChanged();
    }

    [RelayCommand]
    private void RemoveMaterialItem(BidMaterialLineItemViewModel item)
    {
        MaterialLineItems.Remove(item);
        RaiseMaterialTotalsChanged();
    }


    private void SaveValidTasks()
    {
        var validTasks = Tasks
            .Where(t => !string.IsNullOrWhiteSpace(t.Name) && t.Cost >= 0 && t.Sale >= 0)
            .ToList();

        if (validTasks.Count == 0)
        {
            Debug.WriteLine("[SAVE TASKS] No valid tasks to save.");
            return;
        }

        Debug.WriteLine($"[SAVE TASKS] Preparing {validTasks.Count} tasks for save...");

        foreach (var task in validTasks)
            Debug.WriteLine($"    • {task.Name}, Cost: {task.Cost}, Sale: {task.Sale}, Type: {task.Type}");

        Tasks.Clear();
        foreach (var task in validTasks)
            Tasks.Add(task);
    }

    #endregion

    #region Item Management

   

    private async Task NavigateToCreateItemAsync()
    {
        
        await _navigationService.GoToAsync("createitems");
    }

  

    private void AddNewComponent()
    {
        var newModel = new BidComponentLineItemModel
        {
            ItemName = "Select Item",
            Description = "Description",
            Type = "Panel Device",
            Qty = 1,
            UnitCost = 0,
            UnitSale = 0,
            Piped = false,
            InstallType = "Normal",
            InstallLocation = "Warehouse"
        };

        var vm = new BidComponentLineItemViewModel(newModel, LaborOverrides, this)
        {
            InstallTypeOptions = InstallTypeOptions,
            InstallLocationOptions = InstallLocationOptions,
            AvailableItems = AvailableItems,
            RaiseComponentTotalsChanged = RaiseComponentTotalsChanged
        };
        ComponentLineItems.Add(vm);
        
    }

    partial void OnMaterialMarkupChanged(decimal value)
    {
        foreach (var item in ComponentLineItems)
        {
            item.ApplyGlobalMarkup(value);
        }

        RaiseComponentTotalsChanged();
    }

    



    public async Task LoadItemsAsync()
    {
        var items = await _itemService.GetAllItemsAsync();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AvailableItems.Clear();
            foreach (var item in items)
            {
                Debug.WriteLine($"🧠 Item: {item.Name} - ID: {item.Id}");
                AvailableItems.Add(item);
            }
            
        });
        Debug.WriteLine($"Loaded {items.Count} items from API");
        foreach (var i in items)
            Debug.WriteLine($" → {i.Name}");
    }

    #endregion

    #region Client Management

    public Func<Task>? RequestAddNewClient { get; set; }

    private async Task AddClient()
    {
        if (RequestAddNewClient != null)
            await RequestAddNewClient.Invoke();
        await LoadClientsAsync();
    }

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

    #endregion

    #region Bid Saving

    private async Task NavigateToCreateTasksAsync()
    {
        await _navigationService.GoToAsync("createtasks");
    }

    private async Task SaveBid()
    {
        SaveValidTasks();
        if (string.IsNullOrWhiteSpace(BidNumber) || string.IsNullOrWhiteSpace(ProjectName) || SelectedClient == null)
        {
            await Shell.Current.DisplayAlert("Missing Info", "Please fill in all required fields.", "OK");
            return;
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
            Tasks = AdminTasks.Concat(EngineeringTasks).Select(t => new CreateBidTaskDto
            {
                TaskModelId = t.TaskModelId,
                TaskName = t.Name,
                Cost = t.Cost,
                Sale = t.Sale
            }).ToList(),

            ComponentLineItems = ComponentLineItems.Select(c => new BidComponentLineItemDto
            {
                ItemId = c.Item?.ItemId ?? 0,
                Name = c.ItemName,
                Description = c.Description,
                Type = c.Type,
                Qty = c.Qty,
                UnitCost = c.UnitCost,
                UnitSale = c.UnitSale,
                Piped = c.Piped,
                InstallType = c.InstallType,
                InstallLocation = c.InstallLocation
            }).ToList(),

            WireLineItems = WireLineItems.Select(c => new BidWireLineItemDto
            {
                ItemId = c.Item?.ItemId ?? 0,
                ItemName = c.ItemName,
                Description = c.Description,
                Qty = c.Qty,
                UnitCost = c.UnitCost,
                UnitSale = c.UnitSale
            }).ToList(),

            MaterialLineItems = MaterialLineItems.Select(c => new BidMaterialLineItemDto
            {
                ItemId = c.Item?.ItemId ?? 0,
                ItemName = c.ItemName,
                Description = c.Description,
                Qty = c.Qty,
                UnitCost = c.UnitCost,
                UnitSale = c.UnitSale
            }).ToList()


        };

        var currentTemplateDto = BuildCurrentLaborTemplateDto(); // Method that returns what the user sees now
        var currentJson = JsonSerializer.Serialize(currentTemplateDto, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (_originalLaborTemplateJson != null && _originalLaborTemplateJson != currentJson)
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Unsaved Changes",
                "You've modified the labor template but haven't saved it. Would you like to save it as a new template?",
                "Yes, Save",
                "No"
            );

            if (confirm)
            {
                await SaveTemplateAsync(); // or navigate to the save template screen
            }
        }


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

    #endregion
}