using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace FyreWorksPM.Api.Controllers
{
    // In your BidsController.cs

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
            var bid = await _db.Bids.Include(b => b.Client)
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
            var lastBid = await _db.Bids.OrderByDescending(b => b.Id).FirstOrDefaultAsync();
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
            var lastBid = await _db.Bids.OrderByDescending(b => b.Id).FirstOrDefaultAsync();
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

            var bid = new BidModel
            {
                BidNumber = $"B-{nextNum.ToString("D3")}",
                ProjectName = dto.ProjectName,
                ClientId = dto.ClientId,
                CreatedDate = dto.CreatedDate,
                IsActive = dto.IsActive,
                SiteInfo = siteInfo // 👈 Link the site info
            };

            _db.Bids.Add(bid);
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
            var bid = await _db.Bids.FindAsync(id);
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
            var bid = await _db.Bids.FindAsync(id);
            if (bid == null) return NotFound();

            _db.Bids.Remove(bid);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

    //[ApiController]
    //[Route("api/[controller]")]
    //public class BidsController : ControllerBase
    //{
    //    private readonly ApplicationDbContext _db;

    //    public BidsController(ApplicationDbContext db)
    //    {
    //        _db = db;
    //    }

    //    // GET: api/bids/{id}
    //    [HttpGet("{id}")]
    //    public async Task<ActionResult<BidDto>> GetBid(int id)
    //    {
    //        var bid = await _db.Bids.Include(b => b.Client)
    //                                 .FirstOrDefaultAsync(b => b.Id == id);
    //        if (bid == null) return NotFound();

    //        return Ok(new BidDto
    //        {
    //            Id = bid.Id,
    //            BidNumber = bid.BidNumber,
    //            ProjectName = bid.ProjectName,
    //            ClientId = bid.ClientId,
    //            CreatedDate = bid.CreatedDate
    //        });
    //    }

    //    // POST: api/bids
    //    [HttpPost]
    //    public async Task<ActionResult<BidDto>> CreateBid(CreateBidDto dto)
    //    {
    //        // Get next bid number
    //        var lastBid = await _db.Bids.OrderByDescending(b => b.Id).FirstOrDefaultAsync();
    //        int nextNum = 1;

    //        if (lastBid != null && Regex.Match(lastBid.BidNumber, @"B-(\\d{3})") is Match match && match.Success)
    //        {
    //            nextNum = int.Parse(match.Groups[1].Value) + 1;
    //        }

    //        // Manual mapping
    //        var bid = new BidModel
    //        {
    //            BidNumber = $"B-{nextNum.ToString("D3")}",
    //            ProjectName = dto.ProjectName,
    //            ClientId = dto.ClientId,
    //            CreatedDate = dto.CreatedDate
    //        };

    //        _db.Bids.Add(bid);
    //        await _db.SaveChangesAsync();

    //        // Manual mapping back to DTO
    //        var result = new BidDto
    //        {
    //            Id = bid.Id,
    //            BidNumber = bid.BidNumber,
    //            ProjectName = bid.ProjectName,
    //            ClientId = bid.ClientId,
    //            CreatedDate = bid.CreatedDate
    //        };

    //        return CreatedAtAction(nameof(CreateBid), new { id = bid.Id }, result);
    //    }

    //    // GET: api/bids/next-number
    //    [HttpGet("next-number")]
    //    public async Task<ActionResult<string>> GetNextBidNumber()
    //    {
    //        var lastBid = await _db.Bids
    //            .OrderByDescending(b => b.Id)
    //            .FirstOrDefaultAsync();

    //        int nextNum = 1;

    //        if (lastBid != null && Regex.Match(lastBid.BidNumber, @"B-(\\d{3})") is Match match && match.Success)
    //        {
    //            nextNum = int.Parse(match.Groups[1].Value) + 1;
    //        }

    //        return $"B-{nextNum.ToString("D3")}";
    //    }

    //    // PUT: api/bids/{id}
    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> UpdateBid(int id, [FromBody] UpdateBidDto dto)
    //    {
    //        var bid = await _db.Bids.FindAsync(id);
    //        if (bid == null) return NotFound();

    //        bid.ProjectName = dto.ProjectName;
    //        bid.ClientId = dto.ClientId;
    //        bid.CreatedDate = dto.CreatedDate;

    //        await _db.SaveChangesAsync();
    //        return NoContent();
    //    }

    //    // DELETE: api/bids/{id}
    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> DeleteBid(int id)
    //    {
    //        var bid = await _db.Bids.FindAsync(id);
    //        if (bid == null) return NotFound();

    //        _db.Bids.Remove(bid);
    //        await _db.SaveChangesAsync();
    //        return NoContent();
    //    }

    //}

}
