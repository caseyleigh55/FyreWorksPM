using Microsoft.AspNetCore.Authorization;
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
/// Handles all user-related endpoints including registration, login, and (temporary) listing.
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
    /// 🔐 TEMP: Returns all users. Remove or lock this down in production.
    /// </summary>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _db.Users.ToListAsync();
        return Ok(users);
    }

    /// <summary>
    /// Registers a new user with a securely hashed password.
    /// </summary>
    /// <param name="dto">The user registration data.</param>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid input.");

        // Check for existing username/email
        bool exists = await _db.Users.AnyAsync(u =>
            u.UserModelUsername == dto.Username || u.UserModelEmail == dto.Email);

        if (exists)
            return BadRequest("A user with this username or email already exists.");

        // Hash password using BCrypt (includes salt)
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new UserModel
        {
            UserModelUsername = dto.Username.Trim(),
            UserModelEmail = dto.Email.Trim(),
            UserModelPasswordHash = hashedPassword
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { message = "User registered successfully." });
    }

    /// <summary>
    /// Logs in the user and returns a JWT access token.
    /// </summary>
    /// <param name="dto">Login credentials (username/password).</param>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserModelUsername == dto.Username);

        // Validate credentials
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.UserModelPasswordHash))
            return Unauthorized("Invalid username or password");

        // Load JWT configuration values
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        // Build claims to embed into token
        var authClaims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserModelUsername),
            new Claim(ClaimTypes.NameIdentifier, user.UserModelId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Construct JWT token
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
