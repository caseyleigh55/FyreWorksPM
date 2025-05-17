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
        public async Task<ActionResult<List<SavedTaskDto>>> GetTemplates([FromQuery] TaskType type)
        {
            var templates = await _db.TaskTemplates
                .Where(t => t.TaskModelType == type)
                .Select(t => new SavedTaskDto
                {
                    Id = t.TaskModelId,
                    TaskName = t.TaskModelTaskName,
                    Type = t.TaskModelType,
                    DefaultCost = t.TaskModelDefaultCost,
                    DefaultSale = t.TaskModelDefaultSale
                })
                .ToListAsync();

            return Ok(templates);
        }

        // POST: api/tasks/templates
        [HttpPost("templates")]
        public async Task<ActionResult<SavedTaskDto>> CreateTemplate([FromBody] CreateTaskDto dto)
        {
            var task = new TaskModel
            {
                TaskModelTaskName = dto.TaskName,
                TaskModelType = dto.Type,
                TaskModelDefaultCost = dto.DefaultCost,
                TaskModelDefaultSale = dto.DefaultSale
            };

            _db.TaskTemplates.Add(task);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemplates), new { type = task.TaskModelType }, new SavedTaskDto
            {
                Id = task.TaskModelId,
                TaskName = task.TaskModelTaskName,
                Type = task.TaskModelType,
                DefaultCost = task.TaskModelDefaultCost,
                DefaultSale = task.TaskModelDefaultSale
            });
        }

        // PUT: api/tasks/templates/5
        [HttpPut("templates/{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromBody] CreateTaskDto dto)
        {
            var task = await _db.TaskTemplates.FindAsync(id);
            if (task == null)
                return NotFound();

            task.TaskModelTaskName = dto.TaskName;
            task.TaskModelType = dto.Type;
            task.TaskModelDefaultCost = dto.DefaultCost;
            task.TaskModelDefaultSale = dto.DefaultSale;

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
