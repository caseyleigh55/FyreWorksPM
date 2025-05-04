using System.ComponentModel.DataAnnotations;

namespace FyreWorksPM.DataAccess.DTO;

/// <summary>
/// DTO used when creating a new item via API.
/// </summary>
public class CreateItemDto
{
    /// <summary>
    /// The name/title of the item.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The item description (optional but recommended).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The name of the item type (e.g., "Conduit", "Sensor").
    /// We'll match this server-side to the actual ItemType.
    /// </summary>
    [Required]
    public string ItemTypeName { get; set; } = string.Empty;
}
