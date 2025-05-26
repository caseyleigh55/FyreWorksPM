using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Models;
using FyreWorksPM.Services.BidLineItem;
using FyreWorksPM.ViewModels.Creation;
using System.Collections.ObjectModel;

public partial class BidComponentLineItemViewModel : ObservableObject
{
    
    private readonly BidLaborConfig _laborOverrides;
    public Action? RaiseComponentTotalsChanged { get; set; }
    private readonly CreateBidViewModel _parentViewModel;

    public BidComponentLineItemViewModel(BidComponentLineItemModel item, BidLaborConfig laborOverrides, CreateBidViewModel parent)
    {
        Item = item;
        _laborOverrides = laborOverrides;
        _parentViewModel = parent;
    }

    public ObservableCollection<ItemDto> AvailableItems { get; set; } = new();


    public List<string> InstallTypeOptions { get; set; } = new();
    public List<string> InstallLocationOptions { get; set; } = new();

    public BidComponentLineItemModel Item { get; }

    public int? ItemId
    {
        get => Item.ItemId;
        set
        {
            if (Item.ItemId != value)
            {
                Item.ItemId = value;
                OnPropertyChanged();
            }
        }
    }

    public string ItemName
    {
        get => Item.ItemName;
        set
        {
            if (Item.ItemName != value)
            {
                Item.ItemName = value;
                OnPropertyChanged();
            }
        }
    }

    public string Description
    {
        get => Item.Description;
        set
        {
            if (Item.Description != value)
            {
                Item.Description = value;
                OnPropertyChanged();
            }
        }
    }

    public string Type => Item.Type;
    public int Qty
    {
        get => Item.Qty;
        set
        {
            if (Item.Qty != value)
            {
                Item.Qty = value;
                OnPropertyChanged();
                RaiseComponentTotalsChanged?.Invoke(); // 🎯 fire the update
            }
        }
    }
    private bool unitSaleManuallySet = false;
   
    public void SetGlobalMarkup(decimal markup)
    {
        _parentViewModel.MaterialMarkup = markup;

        if (!unitSaleManuallySet)
        {
            UnitSale = Math.Round(UnitCost * (1 + markup / 100), 2);
        }

        OnPropertyChanged(nameof(IsSaleOverridden));
    }


    /// <summary>
    /// Whether the UnitSale differs from the calculated markup price.
    /// Used to trigger visual highlighting.
    /// </summary>
    public bool IsSaleOverridden =>
        Math.Round(UnitSale, 2) != Math.Round(UnitCost * (1 + _parentViewModel.MaterialMarkup / 100), 2);

    public decimal UnitCost
    {
        get => Item.UnitCost;
        set
        {
            if (Item.UnitCost != value)
            {
                Item.UnitCost = value;
                unitSaleManuallySet = false; // Reset override state when cost changes
                OnPropertyChanged();

                    var markup = _parentViewModel.MaterialMarkup; // Get global markup from parent view model
                    // Automatically apply markup if user hasn't overridden
                    Item.UnitSale = Math.Round(Item.UnitCost * (1 + markup / 100), 2);
                    OnPropertyChanged(nameof(UnitSale));
                

                OnPropertyChanged(nameof(IsSaleOverridden));
                RaiseComponentTotalsChanged?.Invoke();
            }
        }
    }

    public decimal UnitSale
    {
        get => Item.UnitSale;
        set
        {
            if (Item.UnitSale != value)
            {
                Item.UnitSale = value;
                unitSaleManuallySet = true; // User has overridden markup
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSaleOverridden));
                RaiseComponentTotalsChanged?.Invoke();
            }
        }
    }

    /// <summary>
    /// Applies global markup to UnitSale, unless it's been overridden.
    /// Call this when the global % changes.
    /// </summary>
    public void ApplyGlobalMarkup(decimal markupPercent)
    {
        _parentViewModel.MaterialMarkup = markupPercent;

        if (!unitSaleManuallySet)
        {
            Item.UnitSale = Math.Round(UnitCost * (1 + markupPercent / 100), 2);
            OnPropertyChanged(nameof(UnitSale));
        }

        OnPropertyChanged(nameof(IsSaleOverridden));
    }




    public bool Piped
    {
        get => Item.Piped;
        set
        {
            if (Item.Piped != value)
            {
                Item.Piped = value;
                OnPropertyChanged();
            }
        }
    }

    public string InstallType
    {
        get => Item.InstallType;
        set
        {
            if (Item.InstallType != value)
            {
                Item.InstallType = value;
                OnPropertyChanged();
            }
        }
    }

    public string InstallLocation
    {
        get => Item.InstallLocation;
        set
        {
            if (Item.InstallLocation != value)
            {
                Item.InstallLocation = value;
                OnPropertyChanged();
            }
        }
    }

    public int PrewireMinutes
    {
        get
        {
            var loc = Item.InstallLocation?.ToLowerInvariant().Trim();
            var type = Item.InstallType?.Trim();

            if (_laborOverrides?.PrewireMinutes.TryGetValue(loc, out var byType) == true &&
                byType.TryGetValue(type, out var mins))
                return mins;

            return LaborTimeMatrixService.GetMinutes(loc, type) ?? 0;
        }
    }

    public int TrimMinutes
    {
        get
        {
            if (_laborOverrides != null &&
                _laborOverrides.TrimMinutes.TryGetValue(Item.InstallType, out var mins))
            {
                return mins;
            }

            return TrimTimeLookupService.GetMinutes(Item.InstallType);
        }
    }


    public int TotalMinutes => Qty * (PrewireMinutes + TrimMinutes);
    public double TotalHours => Math.Round(TotalMinutes / 60.0, 2);

    [ObservableProperty] private ItemDto selectedItem;
    partial void OnSelectedItemChanged(ItemDto? value)
    {
        if (value == null) return;

        Item.ItemName = value.Name;
        Item.Description = value.Description;
        Item.ItemId = value.Id;
        // Could add more later like default cost suggestions, etc.
        OnPropertyChanged(nameof(ItemName));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(ItemId));
    }


}
