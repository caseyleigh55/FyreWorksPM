﻿using FyreWorksPM.DataAccess.DTO;

public class CreateBidDto
{
    public string BidNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; } = true;


    public SiteInfoDto SiteInfo { get; set; } = new();

    //Admin and Engineering Tasks Section
    public List<CreateBidTaskDto> Tasks { get; set; } = new();

}
