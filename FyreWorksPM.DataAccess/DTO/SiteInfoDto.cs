// FyreWorksPM.DataAccess/DTO/SiteInfoDto.cs

public class SiteInfoDto
{
    public string SiteInfoDtoScopeOfWork { get; set; } = string.Empty;
    public string SiteInfoDtoAddressLine1 { get; set; } = string.Empty;
    public string SiteInfoDtoAddressLine2 { get; set; } = string.Empty;
    public string SiteInfoDtoCity { get; set; } = string.Empty;
    public string SiteInfoDtoState { get; set; } = string.Empty;
    public string SiteInfoDtoZipCode { get; set; } = string.Empty;
    public string SiteInfoDtoParcelNumber { get; set; } = string.Empty;
    public string SiteInfoDtoJurisdiction { get; set; } = string.Empty;
    public double SiteInfoDtoBuildingArea { get; set; }
    public int SiteInfoDtoNumberOfStories { get; set; }
    public string SiteInfoDtoOccupancyGroup { get; set; } = string.Empty;
    public int SiteInfoDtoOccupantLoad { get; set; }
    public string SiteInfoDtoConstructionType { get; set; } = string.Empty;
    public bool SiteInfoDtoIsSprinklered { get; set; }
}
