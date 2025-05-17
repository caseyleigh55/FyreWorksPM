using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace FyreWorksPM.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BidsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public BidsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/bids/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BidDto>> GetBid(int id)
        {
            var bid = await _db.BidInfo.Include(b => b.BidModelClient)
                                     .FirstOrDefaultAsync(b => b.BidModelBidId == id);
            if (bid == null) return NotFound();

            return Ok(new BidDto
            {
                Id = bid.BidModelBidId,
                BidNumber = bid.BidModelBidNumber,
                ProjectName = bid.BidModelProjectName,
                ClientId = bid.BidModelClientId,
                CreatedDate = bid.BidModelCreatedDate
            });
        }

        // GET: api/bids/next-number
        [HttpGet("next-number")]
        public async Task<ActionResult<string>> GetNextBidNumber()
        {
            var lastBid = await _db.BidInfo.OrderByDescending(b => b.BidModelBidId).FirstOrDefaultAsync();
            int nextNum = 1;

            if (lastBid != null && Regex.Match(lastBid.BidModelBidNumber, @"B-(\d{3})") is Match match && match.Success)
            {
                nextNum = int.Parse(match.Groups[1].Value) + 1;
            }

            return $"B-{nextNum.ToString("D3")}";
        }


        [HttpPost]
        public async Task<ActionResult<BidDto>> CreateBid([FromBody] CreateBidDto dto)
        {
            var lastBid = await _db.BidInfo.OrderByDescending(b => b.BidModelBidId).FirstOrDefaultAsync();
            int nextNum = 1;

            if (lastBid != null && Regex.Match(lastBid.BidModelBidNumber, @"B-(\d{3})") is Match match && match.Success)
            {
                nextNum = int.Parse(match.Groups[1].Value) + 1;
            }

            var siteInfo = new SiteInfoModel
            {
                SiteInfoModelScopeOfWork = dto.SiteInfo.ScopeOfWork,
                SiteInfoModelAddressLine1 = dto.SiteInfo.AddressLine1,
                SiteInfoModelAddressLine2 = dto.SiteInfo.AddressLine2,
                SiteInfoModelCity = dto.SiteInfo.City,
                SiteInfoModelState = dto.SiteInfo.State,
                SiteInfoModelZipCode = dto.SiteInfo.ZipCode,
                SiteInfoModelParcelNumber = dto.SiteInfo.ParcelNumber,
                SiteInfoModelJurisdiction = dto.SiteInfo.Jurisdiction,
                SiteInfoModelBuildingArea = dto.SiteInfo.BuildingArea,
                SiteInfoModelNumberOfStories = dto.SiteInfo.NumberOfStories,
                SiteInfoModelOccupancyGroup = dto.SiteInfo.OccupancyGroup,
                SiteInfoModelOccupantLoad = dto.SiteInfo.OccupantLoad,
                SiteInfoModelConstructionType = dto.SiteInfo.ConstructionType,
                SiteInfoModelIsSprinklered = dto.SiteInfo.IsSprinklered
            };

            // 🎯 Task usage only, no creation
            var bidTasks = new List<BidTaskModel>();

            foreach (var t in dto.Tasks)
            {
                var template = await _db.TaskTemplates.FindAsync(t.TaskModelId);
                if (template == null)
                    return BadRequest($"Invalid TaskModelId: {t.TaskModelId}");

                bidTasks.Add(new BidTaskModel
                {
                    BidTaskModelTaskModelId = template.TaskModelId,
                    BidTaskModelCost = t.Cost,
                    BidTaskModelSale = t.Sale
                });
            }

            var bid = new BidModel
            {
                BidModelBidNumber = $"B-{nextNum:D3}",
                BidModelProjectName = dto.ProjectName,
                BidModelClientId = dto.ClientId,
                BidModelCreatedDate = dto.CreatedDate,
                BidModelIsActive = dto.IsActive,
                BidModelSiteInfo = siteInfo,
                BidModelTasks = bidTasks
            };

            _db.BidInfo.Add(bid);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBid), new { id = bid.BidModelBidId }, new BidDto
            {
                Id = bid.BidModelBidId,
                BidNumber = bid.BidModelBidNumber,
                ProjectName = bid.BidModelProjectName,
                ClientId = bid.BidModelClientId,
                CreatedDate = bid.BidModelCreatedDate,
                IsActive = bid.BidModelIsActive
            });
        }



        // PUT: api/bids/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBid(int id, [FromBody] UpdateBidDto dto)
        {
            var bid = await _db.BidInfo.FindAsync(id);
            if (bid == null) return NotFound();

            bid.BidModelProjectName = dto.ProjectName;
            bid.BidModelClientId = dto.ClientId;
            bid.BidModelCreatedDate = dto.CreatedDate;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/bids/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBid(int id)
        {
            var bid = await _db.BidInfo.FindAsync(id);
            if (bid == null) return NotFound();

            _db.BidInfo.Remove(bid);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
