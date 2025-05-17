using FyreWorksPM.DataAccess.Enums;

namespace FyreWorksPM.DataAccess.DTO
{
    public class CreateTaskDto
    {
        public string TaskName { get; set; } = string.Empty;
        public TaskType Type { get; set; }
        public decimal DefaultCost { get; set; }
        public decimal DefaultSale { get; set; }
    }
}
