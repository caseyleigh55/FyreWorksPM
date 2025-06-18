namespace FyreWorksPM.DataAccess.Data.Models
{
    public class BidLaborRateModel
    {
        public int Id { get; set; }
        public int BidLaborTemplateId { get; set; }
        public BidLaborTemplateModel BidLaborTemplate { get; set; }

        public string Role { get; set; } = string.Empty;
        public decimal RegularDirectRate { get; set; }
        public decimal RegularBilledRate { get; set; }
        public decimal OvernightDirectRate { get; set; }
        public decimal OvernightBilledRate { get; set; }
    }

}
