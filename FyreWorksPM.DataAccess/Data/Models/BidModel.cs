using FyreWorksPM.DataAccess.Data.Models;

public class BidModel
{
    public int Id { get; set; }
    public string BidNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public ClientModel Client { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; } = true; // default to true


    //SiteInfo Section
    public SiteInfoModel? SiteInfo { get; set; }
    public int? SiteInfoId { get; set; }


}
