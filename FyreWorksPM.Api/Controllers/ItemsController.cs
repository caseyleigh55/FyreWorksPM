using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.Api.DTO;

namespace FyreWorksPM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ItemsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ================================================
    // ✅ GET: api/items
    // Returns all items with their type name
    // ================================================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAllItems()
    {
        var items = await _db.Items
            .Include(i => i.ItemType)
            .Select(i => new ItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                ItemTypeName = i.ItemType!.Name
            })
            .ToListAsync();

        return Ok(items);
    }

    // ================================================
    // ✅ GET: api/items/{id}
    // Returns one item by ID
    // ================================================
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItem(int id)
    {
        var item = await _db.Items
            .Include(i => i.ItemType)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null) return NotFound();

        var dto = new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            ItemTypeName = item.ItemType!.Name
        };

        return Ok(dto);
    }

    // ================================================
    // ✅ POST: api/items
    // Adds a new item with linked ItemType
    // ================================================
    [HttpPost]
    public async Task<ActionResult> CreateItem([FromBody] CreateItemDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.ItemTypeName))
        {
            return BadRequest("Item name and item type are required.");
        }

        // Check if the item type exists, or create it
        var itemType = await _db.ItemTypes
            .FirstOrDefaultAsync(t => t.Name == dto.ItemTypeName);

        if (itemType == null)
        {
            itemType = new ItemTypeModel
            {
                Name = dto.ItemTypeName,
                Items = new List<ItemModel>() // required to satisfy `required` modifier
            };

            _db.ItemTypes.Add(itemType);
            await _db.SaveChangesAsync();
        }

        var item = new ItemModel
        {
            Name = dto.Name,
            Description = dto.Description,
            ItemTypeId = itemType.Id
        };

        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
    }

    // ================================================
    // ✅ PUT: api/items/{id}
    // Updates an item
    // ================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] CreateItemDto dto)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null) return NotFound();

        var itemType = await _db.ItemTypes.FirstOrDefaultAsync(t => t.Name == dto.ItemTypeName);
        if (itemType == null)
        {
            itemType = new ItemTypeModel
            {
                Name = dto.ItemTypeName,
                Items = new List<ItemModel>() // required to satisfy `required` modifier
            };

            _db.ItemTypes.Add(itemType);
            await _db.SaveChangesAsync();
        }

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.ItemTypeId = itemType.Id;

        _db.Items.Update(item);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // ================================================
    // ✅ DELETE: api/items/{id}
    // Deletes an item by ID
    // ================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null) return NotFound();

        _db.Items.Remove(item);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
