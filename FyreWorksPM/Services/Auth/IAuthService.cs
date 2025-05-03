namespace FyreWorksPM.Services.Auth;

public interface IAuthService
{
    bool IsLoggedIn { get; }

    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();

    public event EventHandler<AuthStateChangedEventArgs> AuthStateChanged;

    Task TryRestoreLoginAsync();
}
