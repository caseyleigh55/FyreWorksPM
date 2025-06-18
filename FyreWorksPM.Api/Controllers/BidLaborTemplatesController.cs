using FyreWorksPM.DataAccess;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;

namespace FyreWorksPM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidLaborTemplatesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public BidLaborTemplatesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost("{bidId}")]
        public IActionResult SaveBidLaborTemplate(int bidId, [FromBody] BidLaborTemplateDto dto)
        {
            var existing = _db.BidLaborTemplates.FirstOrDefault(t => t.BidId == bidId);
            if (existing != null)
            {
                _db.BidLaborTemplates.Remove(existing);
            }

            var entity = new BidLaborTemplateModel
            {
                BidId = bidId,
                TemplateName = dto.TemplateName,
                LaborRates = dto.LaborRates.Select(r => new BidLaborRateModel
                {
                    Role = r.Role,
                    RegularDirectRate = r.RegularDirectRate,
                    RegularBilledRate = r.RegularBilledRate,
                    OvernightDirectRate = r.OvernightDirectRate,
                    OvernightBilledRate = r.OvernightBilledRate
                }).ToList(),

                LocationHours = dto.LocationHours.Select(l => new BidLocationHourModel
                {
                    LocationName = l.LocationName,
                    Normal = l.Normal,
                    Lift = l.Lift,
                    Panel = l.Panel,
                    Pipe = l.Pipe
                }).ToList(),

                IsDefault = dto.IsDefault
            };


            _db.BidLaborTemplates.Add(entity);
            _db.SaveChanges();

            return Ok(entity);
        }

        [HttpGet("by-bid/{bidId}")]
        public IActionResult GetByBidId(int bidId)
        {
            var entity = _db.BidLaborTemplates.FirstOrDefault(t => t.BidId == bidId);
            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var templates = _db.BidLaborTemplates.ToList();
            return Ok(templates);
        }
    }
}
