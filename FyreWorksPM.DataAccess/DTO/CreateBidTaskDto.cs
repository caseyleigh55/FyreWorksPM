using FyreWorksPM.DataAccess.Enums;

namespace FyreWorksPM.DataAccess.DTO
{
    public class CreateBidTaskDto
    {
        public int TaskModelId { get; set; }     // The template being used
        public decimal Cost { get; set; }        // This bid's custom cost
        public decimal Sale { get; set; }        // This bid's custom sale
    }
}
