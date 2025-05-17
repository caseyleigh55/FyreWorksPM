using System.ComponentModel;
using System.Runtime.CompilerServices;
using FyreWorksPM.DataAccess.Data.Models;

/// <summary>
/// Represents a single item in the system.
/// Supports property change notification for UI binding.
/// </summary>
public class ItemModel : INotifyPropertyChanged
{
    /// <summary>
    /// Primary key for the item.
    /// </summary>
    public int ItemModelId { get; set; }

    private string ItemModelname = string.Empty;

    /// <summary>
    /// The name or title of the item.
    /// </summary>
    public string ItemModelName
    {
        get => ItemModelname;
        set => SetProperty(ref ItemModelname, value);
    }

    private string ItemModeldescription = string.Empty;

    /// <summary>
    /// Optional description or additional details.
    /// </summary>
    public string ItemModelDescription
    {
        get => ItemModeldescription;
        set => SetProperty(ref ItemModeldescription, value);
    }

    private decimal ItemModelunitCost = 0;

    ///// <summary>
    ///// Default unit cost of the item.
    ///// This value is pulled into bids when the item is selected,
    ///// but can be overridden on a per-bid basis.
    ///// </summary>
    //public decimal? UnitCost
    //{
    //    get => unitCost;
    //    set => SetProperty(ref unitCost, value);
    //}

    private ItemTypeModel? ItemModelitemType;

    /// <summary>
    /// Navigation property for the item's type.
    /// </summary>
    public ItemTypeModel? ItemModelItemType
    {
        get => ItemModelitemType;
        set => SetProperty(ref ItemModelitemType, value);
    }

    /// <summary>
    /// Foreign key reference to the associated item type.
    /// Nullable in case it's not set yet.
    /// </summary>
    public int? ItemModelItemTypeId { get; set; }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies the UI that a property has changed.
    /// </summary>
    /// <param name="name">The name of the changed property.</param>
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    /// <summary>
    /// Helper method to raise change notifications only when values actually change.
    /// </summary>
    protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
            return false;

        backingField = value;
        OnPropertyChanged(name);
        return true;
    }

    #endregion
}

