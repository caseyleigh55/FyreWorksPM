using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Enums;
using FyreWorksPM.DataAccess.Models;
using FyreWorksPM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;


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
                                     .FirstOrDefaultAsync(b => b.BidId == id);
            if (bid == null) return NotFound();

            return Ok(new BidDto
            {
                Id = bid.BidId,
                BidNumber = bid.BidNumber,
                ProjectName = bid.ProjectName,
                ClientId = bid.ClientId,
                CreatedDate = bid.CreatedDate,
                IsActive = bid.IsActive
            });
        }

        // GET: api/bids/next-number
        [HttpGet("next-number")]
        public async Task<ActionResult<string>> GetNextBidNumber()
        {
            var lastBid = await _db.BidInfo.OrderByDescending(b => b.BidId).FirstOrDefaultAsync();
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
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    Message = "Validation failed",
                    Errors = errorList
                });
            }

            try
            {



                var lastBid = await _db.BidInfo.OrderByDescending(b => b.BidId).FirstOrDefaultAsync();
            int nextNum = 1;

            if (lastBid != null && Regex.Match(lastBid.BidNumber, @"B-(\d{3})") is Match match && match.Success)
            {
                nextNum = int.Parse(match.Groups[1].Value) + 1;
            }

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

            // 🎯 Task usage only, no creation
            var bidTasks = new List<BidTaskModel>();

            foreach (var t in dto.Tasks)
            {
                var template = await _db.TaskTemplates.FindAsync(t.TaskModelId);
                if (template == null)
                    return BadRequest($"Invalid TaskModelId: {t.TaskModelId}");

                bidTasks.Add(new BidTaskModel
                {
                    TaskModelId = template.Id,
                    Cost = t.Cost,
                    Sale = t.Sale
                });
            }

            //
            //
            //
            var bidComponentLineItems = dto.ComponentLineItems.Select(c => new BidComponentLineItemModel
            {
                ItemId = c.ItemId, // keep this if you're referencing an existing item/template
                ItemName = c.Name,
                Description = c.Description,
                Type = c.Type,
                Qty = c.Qty,
                UnitCost = c.UnitCost,
                UnitSale = c.UnitSale,
                Piped = c.Piped,
                InstallType = c.InstallType,
                InstallLocation = c.InstallLocation
            }).ToList();




            //
            //
            //

            var bid = new BidModel
            {
                BidNumber = $"B-{nextNum:D3}",
                ProjectName = dto.ProjectName,
                ClientId = dto.ClientId,
                CreatedDate = dto.CreatedDate,
                IsActive = dto.IsActive,
                SiteInfo = siteInfo,
                Tasks = bidTasks,
                ComponentLineItems = bidComponentLineItems,
                MaterialMarkup = dto.MaterialMarkup,
                AdjustedSaleTotal = dto.AdjustedSaleTotal

            };
            

            _db.BidInfo.Add(bid);
            await _db.SaveChangesAsync();

            var wireLineItems = dto.WireLineItems.Select(c => new BidWireLineItemModel
            {
                ItemId = c.ItemId,
                ItemName = c.ItemName,
                Description = c.Description,
                Qty = c.Qty,
                UnitCost = c.UnitCost,
                UnitSale = c.UnitSale,
                BidId = bid.BidId
            }).ToList();
            var materialLineItems = dto.MaterialLineItems.Select(c => new BidMaterialLineItemModel
            {
                ItemId = c.ItemId,
                ItemName = c.ItemName,
                Description = c.Description,
                Qty = c.Qty,
                UnitCost = c.UnitCost,
                UnitSale = c.UnitSale,
                BidId = bid.BidId
            }).ToList();

            _db.BidWireLineItems.AddRange(wireLineItems);
            _db.BidMaterialLineItems.AddRange(materialLineItems);

            var bidLaborTemplate = new BidLaborTemplateModel
            {
                BidId = bid.BidId,
                LaborRates = dto.BidLaborTemplate?.LaborRates.Select(rate => new BidLaborRateModel
                {
                    Role = rate.Role,
                    RegularDirectRate = rate.RegularDirectRate,
                    RegularBilledRate = rate.RegularBilledRate,
                    OvernightDirectRate = rate.OvernightDirectRate,
                    OvernightBilledRate = rate.OvernightBilledRate
                }).ToList() ?? new(),

                LocationHours = dto.BidLaborTemplate?.LocationHours.Select(loc => new BidLocationHourModel
                {
                    LocationName = loc.LocationName,
                    Normal = loc.Normal,
                    Lift = loc.Lift,
                    Panel = loc.Panel,
                    Pipe = loc.Pipe
                }).ToList() ?? new(),
                TemplateName = "Snapshot", // or set meaningful name
                IsDefault = false
            };
            _db.BidLaborTemplates.Add(bidLaborTemplate);

            var manualLaborHours = dto.ManualLaborHours?.Select(h => new ManualLaborHourModel
            {
                BidId = bid.BidId,
                Role = h.Role,
                Category = h.Category,
                Hours = h.Hours
            }).ToList();

            if (manualLaborHours != null && manualLaborHours.Any())
            {
                _db.ManualLaborHours.AddRange(manualLaborHours);
            }



            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBid), new { id = bid.BidId }, new BidDto
            {
                Id = bid.BidId,
                BidNumber = bid.BidNumber,
                ProjectName = bid.ProjectName,
                ClientId = bid.ClientId,
                CreatedDate = bid.CreatedDate,
                IsActive = bid.IsActive
            });



        }
    catch (Exception ex)
    {
        return BadRequest(new
        {
            Message = "Server error",
            Error = ex.Message,
            Inner = ex.InnerException?.Message
    });
    }
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
