public class CreateBidDto
{
    public string BidNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; } = true;


    public SiteInfoDto SiteInfo { get; set; } = new();
}
