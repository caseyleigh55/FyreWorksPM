using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DTOs;
using FyreWorksPM.Services.Common;
using FyreWorksPM.Services.Navigation;
using System.Net.Http.Json;

namespace FyreWorksPM.ViewModels.Creation;

/// <summary>
/// ViewModel responsible for handling user registration logic from the UI.
/// Refactored for ObservableProperty and RelayCommand usage.
/// </summary>
public partial class CreateUserViewModel : ObservableObject
{
    private readonly ILoadingService _loading;
    private readonly HttpClient _httpClient;
    private readonly INavigationServices _navigation;

    public CreateUserViewModel(HttpClient httpClient, INavigationServices navigation, ILoadingService loading)
    {
        _httpClient = httpClient;
        _navigation = navigation;
        _loading = loading;
    }

    // 🔗 Observable form fields for data binding
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;

    /// <summary>
    /// Called when the Register button is clicked.
    /// Performs validation, sends API call, and navigates on success.
    /// </summary>
    [RelayCommand]
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

    /// <summary>
    /// Navigates user back to login page.
    /// </summary>
    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await _navigation.GoToAsync("//login");
    }
}
