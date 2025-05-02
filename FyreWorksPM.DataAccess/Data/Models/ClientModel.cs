using System.ComponentModel.DataAnnotations;


namespace FyreWorksPM.DataAccess.Data.Models
{
    public class ClientModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = default!;
        public string Contact { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
    }
}
