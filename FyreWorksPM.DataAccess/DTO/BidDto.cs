using FyreWorksPM.DataAccess.DTO;

public class BidDto
{
    public int BidDtoId { get; set; }
    public string BidDtoBidNumber { get; set; } = string.Empty;
    public string BidDtoProjectName { get; set; } = string.Empty;
    public string BidDtoClientName { get; set; } = string.Empty;
    public int BidDtoClientId { get; set; }
    public DateTime BidDtoCreatedDate { get; set; }

    /// <summary>
    /// Indicates whether the bid is active or has been archived/inactivated.
    /// </summary>
    public bool BidDtoIsActive { get; set; } = true;
    public List<CreateBidTaskDto> BidDtoTasks { get; set; } = new();

}
