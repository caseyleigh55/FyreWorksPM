namespace FyreWorksPM.DataAccess.DTO
{
    public class BidLaborTemplateDto
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public bool IsDefault { get; set; }

        public List<BidLaborRateDto> LaborRates { get; set; }
        public List<BidLocationHourDto> LocationHours { get; set; }
    }

}
