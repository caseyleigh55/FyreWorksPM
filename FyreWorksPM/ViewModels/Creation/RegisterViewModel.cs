using System.Net.Http.Json;
using System.Windows.Input;
using FyreWorksPM.DTOs;
using FyreWorksPM.Services.Common;
using FyreWorksPM.Services.Navigation;

namespace FyreWorksPM.ViewModels.Creation;

/// <summary>
/// ViewModel responsible for handling user registration logic from the UI.
/// Uses HttpClient to communicate with the API and NavigationService to switch views.
/// </summary>
public class RegisterViewModel : ViewModelBase
{
    private readonly ILoadingService _loading;
    private readonly HttpClient _httpClient;
    private readonly INavigationService _navigation;

    // =====================
    // 🔗 Bindable Properties
    // =====================

    /// <summary>
    /// Username entered by the user.
    /// </summary>
    public string Username { get => Get<string>(); set => Set(value); }

    /// <summary>
    /// Email address entered by the user.
    /// </summary>
    public string Email { get => Get<string>(); set => Set(value); }

    /// <summary>
    /// Password entered by the user.
    /// </summary>
    public string Password { get => Get<string>(); set => Set(value); }

    /// <summary>
    /// Confirm password field to match validation.
    /// </summary>
    public string ConfirmPassword { get => Get<string>(); set => Set(value); }

    // =====================
    // 🎮 Commands
    // =====================

    /// <summary>
    /// Command triggered when the Register button is clicked.
    /// </summary>
    public ICommand RegisterCommand { get; }

    /// <summary>
    /// Command triggered when the user opts to return to the login screen.
    /// </summary>
    public ICommand GoToLoginCommand { get; }

    // =====================
    // 🔨 Constructor
    // =====================

    /// <summary>
    /// Constructor for RegisterViewModel.
    /// Accepts dependencies for navigation and HTTP communication.
    /// </summary>
    public RegisterViewModel(HttpClient httpClient, INavigationService navigation, ILoadingService loading)
    {
        _httpClient = httpClient;
        _navigation = navigation;
        _loading = loading;

        RegisterCommand = new Command(async () => await RegisterAsync());
        GoToLoginCommand = new Command(async () => await ReturnToLogin());
    }

    // =====================
    // 🚪 Navigation Logic
    // =====================

    /// <summary>
    /// Navigates the user back to the login screen.
    /// </summary>
    private async Task ReturnToLogin()
    {
        await _navigation.GoToAsync("//login");
    }

    // =====================
    // 🧠 Registration Logic
    // =====================

    /// <summary>
    /// Validates user input, sends the registration data to the API,
    /// and navigates back to login upon success.
    /// </summary>
    private async Task RegisterAsync()
    {
        await _loading.ShowAsync();

        try
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "All fields are required", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            var request = new RegisterRequest
            {
                Username = Username.Trim(),
                Email = Email.Trim(),
                Password = Password
            };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7139/api/users/register", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Error", error, "OK");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Success", "User created!", "OK");
            await _navigation.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Oops!", $"Something went wrong:\n{ex.Message}", "OK");
        }
        finally
        {
            await _loading.HideAsync();
        }
    }

}
