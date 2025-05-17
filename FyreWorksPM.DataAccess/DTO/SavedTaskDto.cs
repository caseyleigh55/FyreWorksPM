using FyreWorksPM.DataAccess.Enums;

namespace FyreWorksPM.DataAccess.DTO
{
    public class SavedTaskDto
    {
        public int SavedTaskDtoId { get; set; }
        public string SavedTaskDtoTaskName { get; set; }
        public TaskType SavedTaskDtoType { get; set; }
        public decimal SavedTaskDtoDefaultCost { get; set; }
        public decimal SavedTaskDtoDefaultSale { get; set; }

        public override string ToString()
        {
            return SavedTaskDtoTaskName;
        }

    }


}
