using FyreWorksPM.DataAccess.DTO;

public class CreateBidDto
{
    public string CreateBidDtoBidNumber { get; set; } = string.Empty;
    public string CreateBidDtoProjectName { get; set; } = string.Empty;
    public int CreateBidDtoClientId { get; set; }
    public DateTime CreateBidDtoCreatedDate { get; set; }
    public bool CreateBidDtoIsActive { get; set; } = true;


    public SiteInfoDto CreateBidDtoSiteInfo { get; set; } = new();

    //Admin and Engineering Tasks Section
    public List<CreateBidTaskDto> CreateBidDtoTasks { get; set; } = new();

}
