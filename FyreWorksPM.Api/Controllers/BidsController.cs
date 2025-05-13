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
            var bid = await _db.BidInfo.Include(b => b.Client)
                                     .FirstOrDefaultAsync(b => b.Id == id);
            if (bid == null) return NotFound();

            return Ok(new BidDto
            {
                Id = bid.Id,
                BidNumber = bid.BidNumber,
                ProjectName = bid.ProjectName,
                ClientId = bid.ClientId,
                CreatedDate = bid.CreatedDate
            });
        }

        // GET: api/bids/next-number
        [HttpGet("next-number")]
        public async Task<ActionResult<string>> GetNextBidNumber()
        {
            var lastBid = await _db.BidInfo.OrderByDescending(b => b.Id).FirstOrDefaultAsync();
            int nextNum = 1;

            if (lastBid != null && Regex.Match(lastBid.BidNumber, @"B-(\d{3})") is Match match && match.Success)
            {
                nextNum = int.Parse(match.Groups[1].Value) + 1;
            }

            return $"B-{nextNum.ToString("D3")}";
        }


        [HttpPost]
        public async Task<ActionResult<BidDto>> CreateBid([FromBody] CreateBidDto dto)
        {
            var lastBid = await _db.BidInfo.OrderByDescending(b => b.Id).FirstOrDefaultAsync();
            int nextNum = 1;

            if (lastBid != null && Regex.Match(lastBid.BidNumber, @"B-(\d{3})") is Match match && match.Success)
            {
                nextNum = int.Parse(match.Groups[1].Value) + 1;
            }

            // 🌱 Map SiteInfoDto → SiteInfoModel
            var siteInfo = new SiteInfoModel
            {
                ScopeOfWork = dto.SiteInfo.ScopeOfWork,
                AddressLine1 = dto.SiteInfo.AddressLine1,
                AddressLine2 = dto.SiteInfo.AddressLine2,
                City = dto.SiteInfo.City,
                State = dto.SiteInfo.State,
                ZipCode = dto.SiteInfo.ZipCode,
                ParcelNumber = dto.SiteInfo.ParcelNumber,
                Jurisdiction = dto.SiteInfo.Jurisdiction,
                BuildingArea = dto.SiteInfo.BuildingArea,
                NumberOfStories = dto.SiteInfo.NumberOfStories,
                OccupancyGroup = dto.SiteInfo.OccupancyGroup,
                OccupantLoad = dto.SiteInfo.OccupantLoad,
                ConstructionType = dto.SiteInfo.ConstructionType,
                IsSprinklered = dto.SiteInfo.IsSprinklered
            };

            var debugIncomingDtoTaskCount = dto.Tasks?.Count ?? -1;


            // 🎯 Mapping admin/engineering tasks
            var bidTasks = new List<BidTaskModel>();

            foreach (var t in dto.Tasks)
            {
                TaskModel existingTemplate = null;

                // Only look up/create if TaskModelId is 0
                if (t.TaskModelId == 0)
                {
                    existingTemplate = await _db.TaskTemplates
                        .FirstOrDefaultAsync(tm => tm.TaskName == t.TaskName && tm.Type == t.Type);

                    if (existingTemplate == null)
                    {
                        existingTemplate = new TaskModel
                        {
                            TaskName = t.TaskName,
                            Type = t.Type
                        };

                        _db.TaskTemplates.Add(existingTemplate);
                        await _db.SaveChangesAsync(); // Get new ID
                    }
                }
                else
                {
                    existingTemplate = await _db.TaskTemplates.FindAsync(t.TaskModelId);
                }

                // Create the actual usage entry
                bidTasks.Add(new BidTaskModel
                {
                    TaskModelId = existingTemplate.Id,
                    Cost = t.Cost,
                    Sale = t.Sale
                });
            }

            var bid = new BidModel
            {
                BidNumber = $"B-{nextNum.ToString("D3")}",
                ProjectName = dto.ProjectName,
                ClientId = dto.ClientId,
                CreatedDate = dto.CreatedDate,
                IsActive = dto.IsActive,
                SiteInfo = siteInfo, // 👈 Link the site info
                Tasks = bidTasks,
                
            };
            var debugTaskCount = bid.Tasks.Count;


            _db.BidInfo.Add(bid);
            
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBid), new { id = bid.Id }, new BidDto
            {
                Id = bid.Id,
                BidNumber = bid.BidNumber,
                ProjectName = bid.ProjectName,
                ClientId = bid.ClientId,
                CreatedDate = bid.CreatedDate,
                IsActive = bid.IsActive
                // You could return SiteInfo as well later in BidDto if needed
            });
        }


        // PUT: api/bids/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBid(int id, [FromBody] UpdateBidDto dto)
        {
            var bid = await _db.BidInfo.FindAsync(id);
            if (bid == null) return NotFound();

            bid.ProjectName = dto.ProjectName;
            bid.ClientId = dto.ClientId;
            bid.CreatedDate = dto.CreatedDate;

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
