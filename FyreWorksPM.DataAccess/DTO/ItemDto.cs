namespace FyreWorksPM.DataAccess.DTO;

/// <summary>
/// DTO used for returning item data or editing existing items.
/// </summary>
public class ItemDto
{
    public int ItemDtoId { get; set; }

    public string ItemDtoName { get; set; } = string.Empty;

    public string? ItemDtoDescription { get; set; }

    public string ItemDtoItemTypeName { get; set; } = string.Empty;
}
