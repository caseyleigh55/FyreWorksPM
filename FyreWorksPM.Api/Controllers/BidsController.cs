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
                BidDtoId = bid.BidModelBidId,
                BidDtoBidNumber = bid.BidModelBidNumber,
                BidDtoProjectName = bid.BidModelProjectName,
                BidDtoClientId = bid.BidModelClientId,
                BidDtoCreatedDate = bid.BidModelCreatedDate
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
                SiteInfoModelScopeOfWork = dto.CreateBidDtoSiteInfo.SiteInfoDtoScopeOfWork,
                SiteInfoModelAddressLine1 = dto.CreateBidDtoSiteInfo.SiteInfoDtoAddressLine1,
                SiteInfoModelAddressLine2 = dto.CreateBidDtoSiteInfo.SiteInfoDtoAddressLine2,
                SiteInfoModelCity = dto.CreateBidDtoSiteInfo.SiteInfoDtoCity,
                SiteInfoModelState = dto.CreateBidDtoSiteInfo.SiteInfoDtoState,
                SiteInfoModelZipCode = dto.CreateBidDtoSiteInfo.SiteInfoDtoZipCode,
                SiteInfoModelParcelNumber = dto.CreateBidDtoSiteInfo.SiteInfoDtoParcelNumber,
                SiteInfoModelJurisdiction = dto.CreateBidDtoSiteInfo.SiteInfoDtoJurisdiction,
                SiteInfoModelBuildingArea = dto.CreateBidDtoSiteInfo.SiteInfoDtoBuildingArea,
                SiteInfoModelNumberOfStories = dto.CreateBidDtoSiteInfo.SiteInfoDtoNumberOfStories,
                SiteInfoModelOccupancyGroup = dto.CreateBidDtoSiteInfo.SiteInfoDtoOccupancyGroup,
                SiteInfoModelOccupantLoad = dto.CreateBidDtoSiteInfo.SiteInfoDtoOccupantLoad,
                SiteInfoModelConstructionType = dto.CreateBidDtoSiteInfo.SiteInfoDtoConstructionType,
                SiteInfoModelIsSprinklered = dto.CreateBidDtoSiteInfo.SiteInfoDtoIsSprinklered
            };

            // 🎯 Task usage only, no creation
            var bidTasks = new List<BidTaskModel>();

            foreach (var t in dto.CreateBidDtoTasks)
            {
                var template = await _db.TaskTemplates.FindAsync(t.CreateBidTaskDtoTaskModelId);
                if (template == null)
                    return BadRequest($"Invalid TaskModelId: {t.CreateBidTaskDtoTaskModelId}");

                bidTasks.Add(new BidTaskModel
                {
                    BidTaskModelTaskModelId = template.TaskModelId,
                    BidTaskModelCost = t.CreateBidTaskDtoCost,
                    BidTaskModelSale = t.CreateBidTaskDtoSale
                });
            }

            var bid = new BidModel
            {
                BidModelBidNumber = $"B-{nextNum:D3}",
                BidModelProjectName = dto.CreateBidDtoProjectName,
                BidModelClientId = dto.CreateBidDtoClientId,
                BidModelCreatedDate = dto.CreateBidDtoCreatedDate,
                BidModelIsActive = dto.CreateBidDtoIsActive,
                BidModelSiteInfo = siteInfo,
                BidModelTasks = bidTasks
            };

            _db.BidInfo.Add(bid);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBid), new { id = bid.BidModelBidId }, new BidDto
            {
                BidDtoId = bid.BidModelBidId,
                BidDtoBidNumber = bid.BidModelBidNumber,
                BidDtoProjectName = bid.BidModelProjectName,
                BidDtoClientId = bid.BidModelClientId,
                BidDtoCreatedDate = bid.BidModelCreatedDate,
                BidDtoIsActive = bid.BidModelIsActive
            });
        }



        // PUT: api/bids/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBid(int id, [FromBody] UpdateBidDto dto)
        {
            var bid = await _db.BidInfo.FindAsync(id);
            if (bid == null) return NotFound();

            bid.BidModelProjectName = dto.UpdateBidDtoProjectName;
            bid.BidModelClientId = dto.UpdateBidDtoClientId;
            bid.BidModelCreatedDate = dto.UpdateBidDtoCreatedDate;

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
