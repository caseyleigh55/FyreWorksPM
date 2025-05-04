using FyreWorksPM.DataAccess.Data.Models;

public class BidModel
{
    public int Id { get; set; }
    public string BidNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public ClientModel Client { get; set; }
    public DateTime CreatedDate { get; set; }
}
