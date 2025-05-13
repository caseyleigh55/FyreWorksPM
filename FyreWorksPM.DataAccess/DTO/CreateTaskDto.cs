using FyreWorksPM.DataAccess.Enums;

namespace FyreWorksPM.DataAccess.DTO
{
    public class CreateTaskDto
    {
        public int TaskModelId { get; set; }      // This may be 0 if the task is new
        public string TaskName { get; set; }      // Needed for lookup/creation
        public TaskType Type { get; set; }        // Needed for lookup/creation
        public decimal Cost { get; set; }
        public decimal Sale { get; set; }
    }


}
