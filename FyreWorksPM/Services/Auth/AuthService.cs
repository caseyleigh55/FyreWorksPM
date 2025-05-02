using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FyreWorksPM.Services.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private string? _token;

    public bool IsLoggedIn { get; private set; } = false;

    public event EventHandler<AuthStateChangedEventArgs> AuthStateChanged;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("https://localhost:7139/api/Users/login", new
        {
            Username = username,
            Password = password
        });

        if (!response.IsSuccessStatusCode)
        {
            IsLoggedIn = false;
            AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs(false));
            return false;
        }

        // 🔥 Deserialize the response and extract the JWT
        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<LoginResult>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result?.Token == null)
        {
            IsLoggedIn = false;
            AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs(false));
            return false;
        }

        _token = result.Token;

        // 🔐 Set the token as default Authorization header
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        IsLoggedIn = true;
        AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs(true));
        return true;
    }

    public Task LogoutAsync()
    {
        IsLoggedIn = false;
        _token = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;

        AuthStateChanged?.Invoke(this, new AuthStateChangedEventArgs(false));
        return Task.CompletedTask;
    }

    private class LoginResult
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
