namespace FyreWorksPM.DataAccess.Data.Models;

/// <summary>
/// Represents an application user with login credentials.
/// Stores username, email, and a securely hashed password.
/// </summary>
public class UserModel
{
    /// <summary>
    /// Primary key identifier for the user.
    /// </summary>
    public int UserModelId { get; set; }

    /// <summary>
    /// Unique username used for login.
    /// </summary>
    public string UserModelUsername { get; set; } = default!;

    /// <summary>
    /// User's email address. Should be unique.
    /// </summary>
    public string UserModelEmail { get; set; } = default!;

    /// <summary>
    /// Securely hashed password using BCrypt or other hashing algorithms.
    /// </summary>
    public string UserModelPasswordHash { get; set; } = default!;
}
