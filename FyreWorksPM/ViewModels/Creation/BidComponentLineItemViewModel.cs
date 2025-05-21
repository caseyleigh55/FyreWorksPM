using CommunityToolkit.Mvvm.ComponentModel;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.Models;
using FyreWorksPM.Services.BidLineItem;

public class BidComponentLineItemViewModel : ObservableObject
{
    public BidComponentLineItemModel Item { get; }
    private readonly BidLaborConfig _laborOverrides;

    public BidComponentLineItemViewModel(BidComponentLineItemModel item, BidLaborConfig laborOverrides)
    {
        Item = item;
        _laborOverrides = laborOverrides;
    }

    public List<string> InstallTypeOptions { get; set; } = new();
    public List<string> InstallLocationOptions { get; set; } = new();


    public string ItemName => Item.ItemName;
    public string Description => Item.Description;
    public string Type => Item.Type;
    public int Qty => Item.Qty;
    public decimal UnitCost => Item.UnitCost;
    public decimal UnitSale => Item.UnitSale;

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


}
