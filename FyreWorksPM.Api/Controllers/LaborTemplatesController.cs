using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FyreWorksPM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LaborTemplatesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public LaborTemplatesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ========================================================
        // POST: api/LaborTemplates
        // Create a new labor template from DTO
        // ========================================================
        [HttpPost]
        public async Task<ActionResult<LaborTemplateDto>> CreateTemplateAsync([FromBody] CreateLaborTemplateDto dto)
        {
            var exists = await _db.LaborTemplates.AnyAsync(t => t.TemplateName == dto.TemplateName);
            if (exists)
                return BadRequest($"A template named '{dto.TemplateName}' already exists.");

            if (dto.IsDefault)
            {
                var currentDefault = await _db.LaborTemplates.FirstOrDefaultAsync(t => t.IsDefault);
                if (currentDefault != null)
                {
                    currentDefault.IsDefault = false;
                    _db.LaborTemplates.Update(currentDefault);
                }
            }

            var templateId = new int();
            var template = new LaborTemplateModel
            {
                Id = new int(),
                TemplateName = dto.TemplateName,
                IsDefault = dto.IsDefault
            };
            template.LaborRates = dto.LaborRates.Select(r => new LaborRateModel
            {
                Role = r.Role,
                RegularDirectRate = r.RegularDirectRate,
                RegularBilledRate = r.RegularBilledRate,
                OvernightDirectRate = r.OvernightDirectRate,
                OvernightBilledRate = r.OvernightBilledRate,
                LaborTemplateId = templateId
            }).ToList();
            template.LocationHours = dto.LocationHours.Select(h => new LocationHourModel
            {
                LocationName = h.LocationName,
                Normal = h.Normal,
                Lift = h.Lift,
                Panel = h.Panel,
                Pipe = h.Pipe,
                LaborTemplateId = templateId
            }).ToList();
            
            

            _db.LaborTemplates.Add(template);
            await _db.SaveChangesAsync();

            var result = new LaborTemplateDto
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                IsDefault = template.IsDefault,
                LaborRates = template.LaborRates.Select(r => new LaborRateDto
                {
                    Role = r.Role,
                    RegularDirectRate = r.RegularDirectRate,
                    RegularBilledRate = r.RegularBilledRate,
                    OvernightDirectRate = r.OvernightDirectRate,
                    OvernightBilledRate = r.OvernightBilledRate
                }).ToList(),
                LocationHours = template.LocationHours.Select(h => new LocationHourDto
                {
                    LocationName = h.LocationName,
                    Normal = h.Normal,
                    Lift = h.Lift,
                    Panel = h.Panel,
                    Pipe = h.Pipe
                }).ToList()
            };

            return Created($"api/LaborTemplates/{template.Id}", result);

        }

        // ========================================================
        // GET: api/LaborTemplates/{id}
        // Get a template by its ID
        // ========================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<LaborTemplateDto>> GetTemplateByIdAsync(int id)
        {
            var template = await _db.LaborTemplates
                .Include(t => t.LaborRates)
                .Include(t => t.LocationHours)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null) return NotFound();

            var dto = new LaborTemplateDto
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                IsDefault = template.IsDefault,
                LaborRates = template.LaborRates.Select(r => new LaborRateDto
                {
                    Role = r.Role,
                    RegularDirectRate = r.RegularDirectRate,
                    RegularBilledRate = r.RegularBilledRate,
                    OvernightDirectRate = r.OvernightDirectRate,
                    OvernightBilledRate = r.OvernightBilledRate
                }).ToList(),
                LocationHours = template.LocationHours.Select(h => new LocationHourDto
                {
                    LocationName = h.LocationName,
                    Normal = h.Normal,
                    Lift = h.Lift,
                    Panel = h.Panel,
                    Pipe = h.Pipe
                }).ToList()
            };
            return Ok(dto);
        }

        // ========================================================
        // GET: api/LaborTemplates
        // Get all labor templates
        // ========================================================
        [HttpGet]
        public async Task<ActionResult<List<LaborTemplateDto>>> GetAllTemplatesAsync()
        {
            var templates = await _db.LaborTemplates
                .Include(t => t.LaborRates)
                .Include(t => t.LocationHours)
                .ToListAsync();

            return Ok(templates.Select(template => new LaborTemplateDto
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                IsDefault = template.IsDefault,
                LaborRates = template.LaborRates.Select(r => new LaborRateDto
                {
                    Role = r.Role,
                    RegularDirectRate = r.RegularDirectRate,
                    RegularBilledRate = r.RegularBilledRate,
                    OvernightDirectRate = r.OvernightDirectRate,
                    OvernightBilledRate = r.OvernightBilledRate
                }).ToList(),
                LocationHours = template.LocationHours.Select(h => new LocationHourDto
                {
                    LocationName = h.LocationName,
                    Normal = h.Normal,
                    Lift = h.Lift,
                    Panel = h.Panel,
                    Pipe = h.Pipe
                }).ToList()
            }));
        }

        // ========================================================
        // Put: api/LaborTemplates/{id}
        // Updates a template by ID
        // ========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateLaborTemplateDto dto)
        {
            var template = await _db.LaborTemplates
                .Include(t => t.LaborRates)
                .Include(t => t.LocationHours)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template is null)
                return NotFound();

            // Update core fields
            template.TemplateName = dto.TemplateName;
            template.IsDefault = dto.IsDefault;

            // Swap default status if needed
            if (dto.IsDefault)
            {
                var currentDefault = await _db.LaborTemplates
                    .Where(t => t.IsDefault && t.Id != id)
                    .FirstOrDefaultAsync();

                if (currentDefault is not null)
                {
                    currentDefault.IsDefault = false;
                    _db.LaborTemplates.Update(currentDefault);
                }
            }

            // Clear old entries
            _db.LocationHours.RemoveRange(template.LocationHours);
            _db.LaborRates.RemoveRange(template.LaborRates);

            // Add new location hours
            template.LocationHours = dto.LocationHours.Select(h => new LocationHourModel
            {
                LocationName = h.LocationName,
                Normal = h.Normal,
                Lift = h.Lift,
                Panel = h.Panel,
                Pipe = h.Pipe,
                LaborTemplateId = id
            }).ToList();

            // Add new labor rates
            template.LaborRates = dto.LaborRates.Select(r => new LaborRateModel
            {
                Role = r.Role,
                RegularDirectRate = r.RegularDirectRate,
                RegularBilledRate = r.RegularBilledRate,
                OvernightDirectRate = r.OvernightDirectRate,
                OvernightBilledRate = r.OvernightBilledRate,
                LaborTemplateId = id
            }).ToList();

            _db.LaborTemplates.Update(template);
            await _db.SaveChangesAsync();

            return NoContent();
        }


        // ========================================================
        // DELETE: api/LaborTemplates/{id}
        // Deletes a template by ID
        // ========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplateAsync(Guid id)
        {
            var template = await _db.LaborTemplates.FindAsync(id);
            if (template == null) return NotFound();

            _db.LaborTemplates.Remove(template);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
