namespace FyreWorksPM.DataAccess.DTO
{
    public class BidLocationHourDto
    {
        public int Id { get; set; }
        public string LocationName { get; set; }
        public decimal Normal { get; set; }
        public decimal Lift { get; set; }
        public decimal Panel { get; set; }
        public decimal Pipe { get; set; }
    }

}
