using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FyreWorksPM.DataAccess.Data.Models
{
    public class BidLaborTemplateModel
    {
        [Key]
        public int Id { get; set; }
        public int BidId { get; set; }
        [ForeignKey("BidId")]
        public BidModel Bid { get; set; }

        public string TemplateName { get; set; } = string.Empty;
        public bool IsDefault { get; set; }

        public List<BidLaborRateModel> LaborRates { get; set; } = new();
        public List<BidLocationHourModel> LocationHours { get; set; } = new();
    }

}
