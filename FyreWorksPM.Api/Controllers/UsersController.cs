using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
    private readonly IConfiguration _configuration;

    public UsersController(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
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
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid input.");
        }

        bool exists = await _db.Users.AnyAsync(u =>
            u.Username == dto.Username || u.Email == dto.Email);

        if (exists)
        {
            return BadRequest("A user with this username or email already exists.");
        }

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

    /// <summary>
    /// Logs in a user and returns a JWT token.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid username or password");
        }

        // JWT Config
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var authClaims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: null,
            expires: DateTime.UtcNow.AddHours(2),
            claims: authClaims,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString,
            expiration = token.ValidTo
        });
    }
}
