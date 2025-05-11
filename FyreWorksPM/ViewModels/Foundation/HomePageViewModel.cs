using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.Services.Auth;

namespace FyreWorksPM.ViewModels.Foundation;

/// <summary>
/// ViewModel for the main/home page.
/// Handles logout functionality and main app transitions.
/// </summary>
public partial class HomePageViewModel : ObservableObject
{
    private readonly IAuthService _auth;

    public HomePageViewModel(IAuthService auth)
    {
        _auth = auth;
    }

    /// <summary>
    /// Command to log the user out and return to the login screen.
    /// </summary>
    [RelayCommand]
    private async Task LogOutAsync() => await _auth.LogoutAsync();
}
