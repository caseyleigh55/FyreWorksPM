namespace FyreWorksPM.DataAccess.Data.Models;

/// <summary>
/// Represents a category or type of item in the system (e.g., Wire, Conduit, Sensor).
/// Used to group items logically and relationally.
/// </summary>
public class ItemTypeModel
{
    /// <summary>
    /// Primary key for the item type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name or label of the item type (must be unique and descriptive).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property for items associated with this type.
    /// </summary>
    public virtual ICollection<ItemModel> Items { get; set; } = new List<ItemModel>();

}
