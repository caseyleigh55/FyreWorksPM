public class BidDto
{
    public int Id { get; set; }
    public string BidNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public DateTime CreatedDate { get; set; }
}
