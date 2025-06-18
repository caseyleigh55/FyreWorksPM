namespace FyreWorksPM.DataAccess.Data.Models
{
    public class ManualLaborHourModel
    {
        public int Id { get; set; }
        public int BidId { get; set; }
        public BidModel Bid { get; set; }

        public string Role { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // e.g. "Trim", "Demo", "Test"
        public decimal Hours { get; set; }
    }

}
