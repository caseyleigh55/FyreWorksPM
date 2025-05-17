using FyreWorksPM.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FyreWorksPM.DataAccess.Data.Models
{
    [Table("BidTasks")] // Changed to make the table name more accurate & avoid collision with other "Tasks"
    public class BidTaskModel
    {
        [Key]
        public int BidTaskModelId { get; set; }

        // --- Relationships ---

        // FK to Bid
        [ForeignKey(nameof(BidTaskModelBid))]
        public int BidTaskModelBidId { get; set; }
        public virtual BidModel BidTaskModelBid { get; set; } = null!;

        // FK to Task Template
        [ForeignKey(nameof(BidTaskModelTask))]
        public int BidTaskModelTaskModelId { get; set; }
        public virtual TaskModel BidTaskModelTask { get; set; } = null!;

        // --- Task-specific pricing for this bid ---
        [Column(TypeName = "decimal(18,2)")]
        public decimal BidTaskModelCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BidTaskModelSale { get; set; }
    }
}
