namespace FyreWorksPM.DataAccess.Data.Models
{
    public class BidLocationHourModel
    {
        public int Id { get; set; }
        public int BidLaborTemplateId { get; set; }
        public BidLaborTemplateModel BidLaborTemplate { get; set; }

        public string LocationName { get; set; } = string.Empty;
        public decimal Normal { get; set; }
        public decimal Lift { get; set; }
        public decimal Panel { get; set; }
        public decimal Pipe { get; set; }
    }

}
