namespace FyreWorksPM.DataAccess.DTO
{
    public class BidLaborRateDto
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public decimal RegularDirectRate { get; set; }
        public decimal RegularBilledRate { get; set; }
        public decimal OvernightDirectRate { get; set; }
        public decimal OvernightBilledRate { get; set; }
    }

}
