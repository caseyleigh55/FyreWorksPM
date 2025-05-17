using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;

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
            .Include(i => i.ItemModelItemType)
            .Select(i => new ItemDto
            {
                ItemDtoId = i.ItemModelId,
                ItemDtoName = i.ItemModelName,
                ItemDtoDescription = i.ItemModelDescription,
                ItemDtoItemTypeName = i.ItemModelItemType!.ItemTypeModelName
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
            .Include(i => i.ItemModelItemType)
            .FirstOrDefaultAsync(i => i.ItemModelId == id);

        if (item == null) return NotFound();

        var dto = new ItemDto
        {
            ItemDtoId = item.ItemModelId,
            ItemDtoName = item.ItemModelName,
            ItemDtoDescription = item.ItemModelDescription,
            ItemDtoItemTypeName = item.ItemModelItemType!.ItemTypeModelName
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
        try
        {
            if (string.IsNullOrWhiteSpace(dto.CreateItemDtoName) || string.IsNullOrWhiteSpace(dto.CreateItemDtoItemTypeName))
            {
                return BadRequest("Item name and item type are required.");
            }

            // Check if the item type exists, or create it
            var itemType = await _db.ItemTypes
                .FirstOrDefaultAsync(t => t.ItemTypeModelName == dto.CreateItemDtoItemTypeName);

            if (itemType == null)
            {
                itemType = new ItemTypeModel
                {
                    ItemTypeModelName = dto.CreateItemDtoItemTypeName,
                    ItemTypeModelItems = new List<ItemModel>() // ensure not null
                };

                _db.ItemTypes.Add(itemType);
                await _db.SaveChangesAsync();
            }

            var item = new ItemModel
            {
                ItemModelName = dto.CreateItemDtoName,
                ItemModelDescription = dto.CreateItemDtoDescription,
                ItemModelItemTypeId = itemType.ItemTypeModelId
            };

            _db.Items.Add(item);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.ItemModelId }, item);
        }
        catch (DbUpdateException dbEx)
        {
            var msg = $"💥 DB Update failed: {dbEx.Message}\n" +
                      $"{dbEx.InnerException?.Message}";
            Console.WriteLine(msg);
            return StatusCode(500, msg);
        }
        catch (Exception ex)
        {
            var msg = $"💥 Unexpected error: {ex.Message}\n" +
                      $"{ex.InnerException?.Message}";
            Console.WriteLine(msg);
            return StatusCode(500, msg);
        }
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

        var itemType = await _db.ItemTypes.FirstOrDefaultAsync(t => t.ItemTypeModelName == dto.CreateItemDtoItemTypeName);
        if (itemType == null)
        {
            itemType = new ItemTypeModel
            {
                ItemTypeModelName = dto.CreateItemDtoItemTypeName,
                ItemTypeModelItems = new List<ItemModel>() // required to satisfy `required` modifier
            };

            _db.ItemTypes.Add(itemType);
            await _db.SaveChangesAsync();
        }

        item.ItemModelName = dto.CreateItemDtoName;
        item.ItemModelDescription = dto.CreateItemDtoDescription;
        item.ItemModelItemTypeId = itemType.ItemTypeModelId;

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
