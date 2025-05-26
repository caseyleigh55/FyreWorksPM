using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.Models;
using FyreWorksPM.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BidModel
{
    [Key]
    public int BidId { get; set; }
    public string BidNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public ClientModel Client { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; } = true; // default to true


    //SiteInfo Section
    public SiteInfoModel? SiteInfo { get; set; }
    public int? SiteInfoId { get; set; }

    // 💼 New hotness: Admin & Engineering Tasks
    public List<BidTaskModel> Tasks { get; set; } = new();
    public List<BidComponentLineItemModel> ComponentLineItems { get; set; } = new();
  
    public List<BidWireLineItemModel> WireLineItems { get; set; } = new();
    
    public List<BidMaterialLineItemModel> MaterialLineItems { get; set; } = new();






}
