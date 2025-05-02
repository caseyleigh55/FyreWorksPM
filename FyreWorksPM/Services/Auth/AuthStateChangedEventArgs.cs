namespace FyreWorksPM.Services.Auth;

/// <summary>
/// Custom EventArgs to carry login state change info.
/// </summary>
public class AuthStateChangedEventArgs : EventArgs
{
    public bool IsLoggedIn { get; }

    public AuthStateChangedEventArgs(bool isLoggedIn)
    {
        IsLoggedIn = isLoggedIn;
    }
}
