using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using FyreWorksPM.DataAccess.Data.Models;

namespace FyreWorksPM.ViewModels;

/// <summary>
/// Represents a single line item in a bid,
/// including quantity, unit cost, markup, and calculated total.
/// </summary>
public class BidWireLineItemModel
{
    public int Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int? ItemId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitCost { get; set; }
    public decimal UnitSale { get; set; }

    // Foreign key relationship
    public int BidId { get; set; }
    public BidModel Bid { get; set; } = null!;
}
