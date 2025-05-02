using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.Api.DTOs;

namespace FyreWorksPM.Api.Controllers;

/// <summary>
/// Handles user-related actions such as registration and user listing (for testing only).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public UsersController(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns a full list of users. 🔐 TEMP FOR DEBUGGING ONLY.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _db.Users.ToListAsync();
        return Ok(users);
    }

    /// <summary>
    /// Registers a new user with hashed password.
    /// </summary>
    /// <param name="dto">Registration details (username, email, password)</param>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid input.");
        }

        // 🔎 Check if user with same username or email exists
        bool exists = await _db.Users.AnyAsync(u =>
            u.Username == dto.Username || u.Email == dto.Email);

        if (exists)
        {
            return BadRequest("A user with this username or email already exists.");
        }

        // 🔐 Secure the password
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new UserModel
        {
            Username = dto.Username.Trim(),
            Email = dto.Email.Trim(),
            PasswordHash = hashedPassword
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { message = "User registered successfully." });
    }
}
