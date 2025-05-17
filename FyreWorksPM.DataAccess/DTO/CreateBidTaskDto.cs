using FyreWorksPM.DataAccess.Enums;

namespace FyreWorksPM.DataAccess.DTO
{
    public class CreateBidTaskDto
    {
        public int CreateBidTaskDtoTaskModelId { get; set; }     // The template being used
        public string CreateBidTaskDtoTaskName { get; set; }
        public decimal CreateBidTaskDtoCost { get; set; }        // This bid's custom cost
        public decimal CreateBidTaskDtoSale { get; set; }        // This bid's custom sale
    }
}
