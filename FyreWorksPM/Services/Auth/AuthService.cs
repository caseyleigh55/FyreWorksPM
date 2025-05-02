using FyreWorksPM.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;

    public bool IsLoggedIn { get; private set; } = false;

    public event EventHandler<AuthStateChangedEventArgs> AuthStateChanged;

    public AuthService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            IsLoggedIn = false;
            AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs ( false ));
            return false;
        }

        IsLoggedIn = true;
        AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs  (true ));
        return true;
    }

    public Task LogoutAsync()
    {
        IsLoggedIn = false;
        AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs (false));
        return Task.CompletedTask;
    }
}
