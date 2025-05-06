public class BidDto
{
    public int Id { get; set; }
    public string BidNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Indicates whether the bid is active or has been archived/inactivated.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
