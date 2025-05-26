using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.Enums;
using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public TasksController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/tasks/templates?type=0
        [HttpGet("templates")]
        public async Task<ActionResult<List<TaskDto>>> GetTemplates([FromQuery] TaskType type)
        {
            var templates = await _db.TaskTemplates
                .Where(t => t.Type == type)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    TaskName = t.TaskName,
                    Type = t.Type,
                    DefaultCost = t.DefaultCost,
                    DefaultSale = t.DefaultSale
                })
                .ToListAsync();

            return Ok(templates);
        }

        // POST: api/tasks/templates
        [HttpPost("templates")]
        public async Task<ActionResult<TaskDto>> CreateTemplate([FromBody] CreateTaskDto dto)
        {
            var task = new TaskModel
            {
                TaskName = dto.TaskName,
                Type = dto.Type,
                DefaultCost = dto.DefaultCost,
                DefaultSale = dto.DefaultSale
            };

            _db.TaskTemplates.Add(task);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemplates), new { type = task.Type }, new TaskDto
            {
                Id = task.Id,
                TaskName = task.TaskName,
                Type = task.Type,
                DefaultCost = task.DefaultCost,
                DefaultSale = task.DefaultSale
            });
        }

        // PUT: api/tasks/templates/5
        [HttpPut("templates/{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromBody] CreateTaskDto dto)
        {
            var task = await _db.TaskTemplates.FindAsync(id);
            if (task == null)
                return NotFound();

            task.TaskName = dto.TaskName;
            task.Type = dto.Type;
            task.DefaultCost = dto.DefaultCost;
            task.DefaultSale = dto.DefaultSale;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/tasks/templates/5
        [HttpDelete("templates/{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var task = await _db.TaskTemplates.FindAsync(id);
            if (task == null)
                return NotFound();

            _db.TaskTemplates.Remove(task);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
