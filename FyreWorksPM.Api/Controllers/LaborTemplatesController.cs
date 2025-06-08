using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

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

        [HttpPost]
        public async Task<ActionResult<LaborTemplateModel>> CreateTemplateAsync([FromBody] LaborTemplateModel template)
        {
            _db.LaborTemplates.Add(template);
            await _db.SaveChangesAsync();
            return Ok(template);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LaborTemplateModel?>> GetTemplateByIdAsync(Guid id)
        {
            var template = await _db.LaborTemplates
                .Include(t => t.LaborRates)
                .Include(t => t.LocationHours)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null) return NotFound();

            return Ok(template);
        }

        [HttpGet]
        public async Task<ActionResult<List<LaborTemplateModel>>> GetAllTemplatesAsync()
        {
            var templates = await _db.LaborTemplates
                .Include(t => t.LaborRates)
                .Include(t => t.LocationHours)
                .ToListAsync();

            return Ok(templates);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplateAsync(Guid id)
        {
            var template = await _db.LaborTemplates.FindAsync(id);
            if (template is null) return NotFound();

            _db.LaborTemplates.Remove(template);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
