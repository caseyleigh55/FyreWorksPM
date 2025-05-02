using FyreWorksPM;
using FyreWorksPM.Services.Auth;

namespace FyreWorksPM;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;

    public AppShell(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await _authService.LogoutAsync();
        Application.Current.MainPage = new LoginShell(_authService);
    }
}
