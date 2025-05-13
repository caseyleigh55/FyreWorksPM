using FyreWorksPM.DataAccess.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace FyreWorksPM.DataAccess.Data.Models
{
    [Table("Tasks")]
    public class BidTaskModel
    {
        public int Id { get; set; }

        public int BidId { get; set; }
        public BidModel Bid { get; set; }

        public int TaskModelId { get; set; }          // New link to global task
        public TaskModel Task { get; set; }           // Nav prop

        public decimal Cost { get; set; }
        public decimal Sale { get; set; }
    }

}
