using System.ComponentModel.DataAnnotations;


namespace FyreWorksPM.DataAccess.Data.Models
{
    public class ClientModel
    {
        [Key]
        public int ClientModelId { get; set; }

        public string ClientModelName { get; set; } = default!;
        public string ClientModelContact { get; set; } = default!;
        public string ClientModelEmail { get; set; } = default!;
        public string ClientModelPhone { get; set; } = default!;
    }
}
