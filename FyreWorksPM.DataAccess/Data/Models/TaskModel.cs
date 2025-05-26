using FyreWorksPM.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;

namespace FyreWorksPM.DataAccess.Data.Models
{
    public class TaskModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TaskName { get; set; } = string.Empty;

        [Required]
        public TaskType Type { get; set; }

        public decimal DefaultCost { get; set; }

        public decimal DefaultSale { get; set; }
    }
}
