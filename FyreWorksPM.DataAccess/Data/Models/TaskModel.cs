using FyreWorksPM.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;

namespace FyreWorksPM.DataAccess.Data.Models
{
    public class TaskModel
    {
        [Key]
        public int TaskModelId { get; set; }

        [Required]
        public string TaskModelTaskName { get; set; } = string.Empty;

        [Required]
        public TaskType TaskModelType { get; set; }

        public decimal TaskModelDefaultCost { get; set; }

        public decimal TaskModelDefaultSale { get; set; }
    }
}
