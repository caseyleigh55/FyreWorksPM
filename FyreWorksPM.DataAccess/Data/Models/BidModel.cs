using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class BidModel
{
    [Key]
    public int BidModelBidId { get; set; }
    public string BidModelBidNumber { get; set; } = string.Empty;
    public string BidModelProjectName { get; set; } = string.Empty;
    public int BidModelClientId { get; set; }
    public ClientModel BidModelClient { get; set; }
    public DateTime BidModelCreatedDate { get; set; }
    public bool BidModelIsActive { get; set; } = true; // default to true


    //SiteInfo Section
    public SiteInfoModel? BidModelSiteInfo { get; set; }
    public int? BidModelSiteInfoId { get; set; }

    // 💼 New hotness: Admin & Engineering Tasks
    public List<BidTaskModel> BidModelTasks { get; set; } = new();
    public List<BidComponentLineItemModel> BidModelComponentLineItems { get; set; } = new();






}
