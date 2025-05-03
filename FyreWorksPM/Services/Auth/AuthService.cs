using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FyreWorksPM.DTOs;
using FyreWorksPM.Services.Auth;

/// <summary>
/// AuthService handles all authentication-related logic:
/// - Logging in with credentials
/// - Storing/restoring JWT token
/// - Notifying subscribers of login state changes
/// </summary>
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private string? _token;

    /// <summary>
    /// Indicates whether the current user is logged in.
    /// </summary>
    public bool IsLoggedIn { get; private set; } = false;

    /// <summary>
    /// Event triggered whenever login state changes (e.g., login/logout).
    /// Used to update UI and navigation state.
    /// </summary>
    public event EventHandler<AuthStateChangedEventArgs>? AuthStateChanged;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Attempts to log in using the provided username and password.
    /// If successful, stores JWT and sets authorization headers.
    /// </summary>
    public async Task<bool> LoginAsync(string username, string password)
    {
        var request = new LoginRequest
        {
            Username = username,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("https://localhost:7139/api/Users/login", request);

        if (!response.IsSuccessStatusCode)
        {
            // Notify failure
            IsLoggedIn = false;
            AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs(false));
            return false;
        }

        // Deserialize token from response
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResult>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result?.Token == null)
        {
            // Notify failure
            IsLoggedIn = false;
            AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs(false));
            return false;
        }

        // 🔐 Success! Store and set token
        _token = result.Token;

        // Save to secure storage
        await SecureStorage.SetAsync("auth_token", _token);

        // Add to HttpClient headers so all future API calls include token
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _token);

        IsLoggedIn = true;
        AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs(true));
        return true;
    }

    /// <summary>
    /// Logs the user out, clears stored token and auth headers.
    /// </summary>
    public async Task LogoutAsync()
    {
        IsLoggedIn = false;
        _token = null;

        // Clear Authorization header and secure storage
        _httpClient.DefaultRequestHeaders.Authorization = null;
        SecureStorage.Remove("auth_token");

        AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs(false));
    }

    /// <summary>
    /// Attempts to restore a previously saved login token from secure storage.
    /// Useful for persistent login behavior on app launch.
    /// </summary>
    public async Task TryRestoreLoginAsync()
    {
        System.Diagnostics.Debug.WriteLine("[AUTH] TryRestoreLoginAsync started");

        var savedToken = await SecureStorage.GetAsync("auth_token");

        if (!string.IsNullOrWhiteSpace(savedToken))
        {
            _token = savedToken;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            IsLoggedIn = true;
            AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs(true));
        }

        System.Diagnostics.Debug.WriteLine("[AUTH] TryRestoreLoginAsync finished");
    }

    /// <summary>
    /// Internal class used for deserializing login response JSON.
    /// </summary>
    private class LoginResult
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
