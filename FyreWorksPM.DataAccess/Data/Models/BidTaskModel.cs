using FyreWorksPM.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FyreWorksPM.DataAccess.Data.Models
{
    [Table("BidTasks")] // Changed to make the table name more accurate & avoid collision with other "Tasks"
    public class BidTaskModel
    {
        [Key]
        public int Id { get; set; }

        // --- Relationships ---

        // FK to Bid
        [ForeignKey(nameof(Bid))]
        public int BidId { get; set; }
        public virtual BidModel Bid { get; set; } = null!;

        // FK to Task Template
        [ForeignKey(nameof(Task))]
        public int TaskModelId { get; set; }
        public virtual TaskModel Task { get; set; } = null!;

        // --- Task-specific pricing for this bid ---
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Sale { get; set; }
    }
}
