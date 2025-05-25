namespace FyreWorksPM.DataAccess.Models;

public class BidComponentLineItemModel
{
    public int Id { get; set; }

    // Shared fields
    public string ItemName { get; set; } = string.Empty;
    public int? ItemId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public int Qty { get; set; }
    public decimal UnitCost { get; set; }
    public decimal UnitSale { get; set; }

    // Labor-specific fields
    public bool Piped { get; set; }
    public string InstallType { get; set; } = string.Empty;
    public string InstallLocation { get; set; } = string.Empty;

    // Relationship
    public int BidId { get; set; }
    public BidModel Bid { get; set; } = null!;
}
