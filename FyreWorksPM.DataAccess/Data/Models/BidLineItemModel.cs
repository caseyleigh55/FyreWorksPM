using System.ComponentModel;
using System.Runtime.CompilerServices;
using FyreWorksPM.DataAccess.Data.Models;

namespace FyreWorksPM.ViewModels;

/// <summary>
/// Represents a single line item in a bid,
/// including quantity, unit cost, markup, and calculated total.
/// </summary>
public class BidLineItemModel : INotifyPropertyChanged
{
    // ==================== Backing Fields ====================
    private int _quantity = 1;
    private decimal _unitCost;
    private decimal _markupPercent;
    private string _itemName = string.Empty;

    private ItemModel? _selectedItem;

    // ==================== Properties ====================

    /// <summary>
    /// Quantity of this line item.
    /// </summary>
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (SetProperty(ref _quantity, value))
                OnPropertyChanged(nameof(TotalCost));
        }
    }

    /// <summary>
    /// Unit price of this line item before markup.
    /// </summary>
    public decimal UnitCost
    {
        get => _unitCost;
        set
        {
            if (SetProperty(ref _unitCost, value))
                OnPropertyChanged(nameof(TotalCost));
        }
    }

    /// <summary>
    /// Percent markup applied to base cost.
    /// </summary>
    public decimal MarkupPercent
    {
        get => _markupPercent;
        set
        {
            if (SetProperty(ref _markupPercent, value))
                OnPropertyChanged(nameof(TotalCost));
        }
    }

    /// <summary>
    /// Display name of this item (autofilled when selected from library).
    /// </summary>
    public string ItemName
    {
        get => _itemName;
        set => SetProperty(ref _itemName, value);
    }

    /// <summary>
    /// Reference to selected item from the library (if used).
    /// When set, it autofills ItemName and UnitCost.
    /// </summary>
    public ItemModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value) && value != null)
            {
                // ✅ Autofill name and cost from selected item
                ItemName = value.ItemModelName;
                //UnitCost = value.UnitCost;
            }
        }
    }

    /// <summary>
    /// Calculated total cost, including quantity and markup.
    /// </summary>
    public decimal TotalCost =>
        Quantity * UnitCost * (1 + (MarkupPercent / 100));

    // ==================== INotifyPropertyChanged ====================
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
            return false;

        backingField = value;
        OnPropertyChanged(name);
        return true;
    }
}
