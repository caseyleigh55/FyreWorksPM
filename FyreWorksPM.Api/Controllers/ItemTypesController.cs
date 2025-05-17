using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemTypesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ItemTypesController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ================================================
    // ✅ GET: api/itemtypes
    // Returns all item types
    // ================================================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemTypeDto>>> GetAllItemTypes()
    {
        var types = await _db.ItemTypes
            .Select(t => new ItemTypeDto
            {
                Id = t.ItemTypeModelId,
                Name = t.ItemTypeModelName
            })
            .ToListAsync();

        return Ok(types);
    }

    // ================================================
    // ✅ GET: api/itemtypes/{id}
    // Returns a single item type by ID
    // ================================================
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemTypeDto>> GetItemType(int id)
    {
        var type = await _db.ItemTypes.FindAsync(id);

        if (type == null)
            return NotFound();

        var dto = new ItemTypeDto
        {
            Id = type.ItemTypeModelId,
            Name = type.ItemTypeModelName
        };

        return Ok(dto);
    }

    // ================================================
    // ✅ POST: api/itemtypes
    // Adds a new item type
    // ================================================
    [HttpPost]
    public async Task<ActionResult> CreateItemType([FromBody] CreateItemTypeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var exists = await _db.ItemTypes.AnyAsync(t => t.ItemTypeModelName == dto.Name);
        if (exists)
            return Conflict("An item type with that name already exists.");

        var itemType = new ItemTypeModel
        {
            ItemTypeModelName = dto.Name,
            ItemTypeModelItems = new List<ItemModel>() // 🚨 Required because of 'required' modifier
        };

        _db.ItemTypes.Add(itemType);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetItemType), new { id = itemType.ItemTypeModelId }, itemType);
    }

    // ================================================
    // ✅ PUT: api/itemtypes/{id}
    // Updates an existing item type
    // ================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItemType(int id, [FromBody] CreateItemTypeDto dto)
    {
        var itemType = await _db.ItemTypes.FindAsync(id);
        if (itemType == null)
            return NotFound();

        itemType.ItemTypeModelName = dto.Name;

        _db.ItemTypes.Update(itemType);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // ================================================
    // ✅ DELETE: api/itemtypes/{id}
    // Deletes an item type (if no items are linked)
    // ================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItemType(int id)
    {
        var itemType = await _db.ItemTypes
            .Include(t => t.ItemTypeModelItems)
            .FirstOrDefaultAsync(t => t.ItemTypeModelId == id);

        if (itemType == null)
            return NotFound();

        if (itemType.ItemTypeModelItems.Any())
            return BadRequest("Cannot delete item type with existing items.");

        _db.ItemTypes.Remove(itemType);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
