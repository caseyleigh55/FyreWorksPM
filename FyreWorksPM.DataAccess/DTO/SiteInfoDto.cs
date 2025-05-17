// FyreWorksPM.DataAccess/DTO/SiteInfoDto.cs

public class SiteInfoDto
{
    public string ScopeOfWork { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string ParcelNumber { get; set; } = string.Empty;
    public string Jurisdiction { get; set; } = string.Empty;
    public double BuildingArea { get; set; }
    public int NumberOfStories { get; set; }
    public string OccupancyGroup { get; set; } = string.Empty;
    public int OccupantLoad { get; set; }
    public string ConstructionType { get; set; } = string.Empty;
    public bool IsSprinklered { get; set; }
}
