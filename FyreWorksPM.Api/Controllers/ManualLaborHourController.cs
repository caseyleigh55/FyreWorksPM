using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;

namespace FyreWorksPM.Api.Controllers
{
    [ApiController]
    [Route("api/manualLaborHours")]
    public class ManualLaborHourController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ManualLaborHourController(ApplicationDbContext context)
        {
            _db = context;
        }

        [HttpGet("{bidId}")]
        public IActionResult GetManualHours(int bidId)
        {
            var hours = _db.ManualLaborHours
                .Where(h => h.BidId == bidId)
                .ToList();

            return Ok(hours);
        }

        [HttpPost("{bidId}")]
        public IActionResult SaveManualHours(int bidId, [FromBody] List<ManualLaborHourDto> dtos)
        {
            var existing = _db.ManualLaborHours.Where(h => h.BidId == bidId);
            _db.ManualLaborHours.RemoveRange(existing);

            var newHours = dtos.Select(dto => new ManualLaborHourModel
            {
                Id = new int(),
                BidId = bidId,
                Role = dto.Role,
                Category = dto.Category,
                Hours = dto.Hours
            }).ToList();

            _db.ManualLaborHours.AddRange(newHours);
            _db.SaveChanges();

            return Ok();
        }
    }
}

