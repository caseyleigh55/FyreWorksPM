using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.DataAccess.Data.Models
{
    public class BidMaterialLineItemModel
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
}
