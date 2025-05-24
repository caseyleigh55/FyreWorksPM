using CommunityToolkit.Mvvm.ComponentModel;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Models;
using FyreWorksPM.Services.BidLineItem;
using System.Collections.ObjectModel;

public partial class BidComponentLineItemViewModel : ObservableObject
{
    public BidComponentLineItemModel Item { get; }
    private readonly BidLaborConfig _laborOverrides;
    public Action? RaiseComponentTotalsChanged { get; set; }


    public BidComponentLineItemViewModel(BidComponentLineItemModel item, BidLaborConfig laborOverrides)
    {
        Item = item;
        _laborOverrides = laborOverrides;        
    }

    public ObservableCollection<ItemDto> AvailableItems { get; set; } = new();


    public List<string> InstallTypeOptions { get; set; } = new();
    public List<string> InstallLocationOptions { get; set; } = new();


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
    public int Qty => Item.Qty;
    public decimal UnitCost
    {
        get => Item.UnitCost;
        set
        {
            if (Item.UnitCost != value)
            {
                Item.UnitCost = value;
                OnPropertyChanged();
                RaiseComponentTotalsChanged?.Invoke(); // 🔥 Fire the total update
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
                OnPropertyChanged();
                RaiseComponentTotalsChanged?.Invoke();
            }
        }
    }


    public bool Piped => Item.Piped;
    public string InstallType => Item.InstallType;
    public string InstallLocation => Item.InstallLocation;

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
        // Could add more later like default cost suggestions, etc.
        OnPropertyChanged(nameof(ItemName));
        OnPropertyChanged(nameof(Description));
    }


}
