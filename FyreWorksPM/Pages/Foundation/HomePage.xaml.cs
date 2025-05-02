using FyreWorksPM.Services.Auth;
namespace FyreWorksPM.Pages.Foundation;



/// <summary>
/// Home page displayed after successful login.
/// </summary>
public partial class HomePage : ContentPage
{
    private readonly IAuthService _authService;

    public HomePage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // Get AuthService from DI
        var authService = App.Current.Handler.MauiContext.Services.GetService<IAuthService>();

        if (authService is not null)
            await authService.LogoutAsync();
        // 🔁 Navigate to the login route
        // 🔄 Completely replace the app's MainPage with a *fresh shell*
        Application.Current.MainPage = new LoginShell(authService);
    }


}
