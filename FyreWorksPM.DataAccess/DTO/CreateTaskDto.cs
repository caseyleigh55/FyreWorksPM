using FyreWorksPM.DataAccess.Enums;

namespace FyreWorksPM.DataAccess.DTO
{
    public class CreateTaskDto
    {
        public string CreateTaskDtoTaskName { get; set; } = string.Empty;
        public TaskType CreateTaskDtoType { get; set; }
        public decimal CreateTaskDtoDefaultCost { get; set; }
        public decimal CreateTaskDtoDefaultSale { get; set; }
    }
}
