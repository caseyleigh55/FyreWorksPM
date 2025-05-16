using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.Services.Auth;
using FyreWorksPM.Services.Common;
using FyreWorksPM.Services.Navigation;

namespace FyreWorksPM.ViewModels.Foundation;

/// <summary>
/// ViewModel responsible for handling user login logic.
/// Connects the login UI to AuthService and controls Shell navigation.
/// </summary>
public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    private readonly INavigationServices _nav;
    private readonly ILoadingService _loading;

    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;

    public LoginViewModel(IAuthService auth, INavigationServices nav, ILoadingService loading)
    {
        _auth = auth;
        _nav = nav;
        _loading = loading;
    }

    /// <summary>
    /// Clears the form fields, useful on logout or reset.
    /// </summary>
    public void Reset()
    {
        Username = string.Empty;
        Password = string.Empty;
    }

    /// <summary>
    /// Validates input, calls AuthService, and switches to the authenticated AppShell if successful.
    /// </summary>
    /// <returns>True if login succeeded, false otherwise.</returns>
    [RelayCommand]
    public async Task<bool> LoginAsync()
    {
        await _loading.ShowAsync();
        try
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await App.Current.MainPage.DisplayAlert("Login Failed", "Enter username and password.", "OK");
                return false;
            }

            bool success = await _auth.LoginAsync(Username, Password);
            if (!success)
            {
                await App.Current.MainPage.DisplayAlert("Login Failed", "Invalid credentials.", "OK");
                return false;
            }

            // ✅ Successful login — switch to authenticated shell
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Application.Current.MainPage = App.Services.GetRequiredService<AppShell>();
            });

            await Shell.Current.GoToAsync("//home");
            return true;
        }
        finally
        {
            await _loading.HideAsync();
        }
    }

    /// <summary>
    /// Navigates to the registration page.
    /// </summary>
    [RelayCommand]
    public async Task GoToRegisterAsync() => await _nav.GoToAsync("//register");
}