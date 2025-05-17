// FyreWorksPM.Api/Models/SiteInfoModel.cs

public class SiteInfoModel
{
    public int SiteInfoModelId { get; set; }

    public string SiteInfoModelScopeOfWork { get; set; } = string.Empty;
    public string SiteInfoModelAddressLine1 { get; set; } = string.Empty;
    public string SiteInfoModelAddressLine2 { get; set; } = string.Empty;
    public string SiteInfoModelCity { get; set; } = string.Empty;
    public string SiteInfoModelState { get; set; } = string.Empty;
    public string SiteInfoModelZipCode { get; set; } = string.Empty;
    public string SiteInfoModelParcelNumber { get; set; } = string.Empty;
    public string SiteInfoModelJurisdiction { get; set; } = string.Empty;
    public double SiteInfoModelBuildingArea { get; set; }
    public int SiteInfoModelNumberOfStories { get; set; }
    public string SiteInfoModelOccupancyGroup { get; set; } = string.Empty;
    public int SiteInfoModelOccupantLoad { get; set; }
    public string SiteInfoModelConstructionType { get; set; } = string.Empty;
    public bool SiteInfoModelIsSprinklered { get; set; }
}
