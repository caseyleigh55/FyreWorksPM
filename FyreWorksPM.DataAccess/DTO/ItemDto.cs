namespace FyreWorksPM.DataAccess.DTO;

/// <summary>
/// DTO used for returning item data or editing existing items.
/// </summary>
public class ItemDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string ItemTypeName { get; set; } = string.Empty;
}
