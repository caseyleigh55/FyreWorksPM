using Microsoft.Maui.Controls;
using FyreWorksPM.Pages.Foundation;
using FyreWorksPM.Pages.Creation;
using FyreWorksPM.Services.Auth;

namespace FyreWorksPM;

/// <summary>
/// Shell used for unauthenticated users (Login/Register only).
/// This shell is manually composed using DI pages.
/// </summary>
public partial class LoginShell : Shell
{
    private readonly IAuthService _authService;

    public LoginShell(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        InitializeShell();
        _authService.AuthStateChanged += OnAuthStateChanged;
    }

    /// <summary>
    /// Sets up the shell layout programmatically using DI-resolved pages.
    /// </summary>
    private void InitializeShell()
    {
        // ✅ Register routes for clean Shell navigation
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));

        // Create login and register pages via DI
        var loginPage = App.Services.GetRequiredService<LoginPage>();
        var registerPage = App.Services.GetRequiredService<RegisterPage>();

        // Setup Login tab
        Items.Add(new ShellContent
        {
            Title = "Login",
            Content = loginPage,
            Route = "login" // ✅ Ensure Shell recognizes this route
        });

        // Setup Register tab
        Items.Add(new ShellContent
        {
            Title = "Register",
            Content = registerPage,
            Route = "register" // ✅ Ensure Shell recognizes this route
        });
    }


    /// <summary>
    /// Swap shell once user successfully logs in.
    /// </summary>
    private void OnAuthStateChanged(object sender, AuthStateChangedEventArgs e)
    {
        if (e.IsLoggedIn)
        {
            // Switch to the authenticated shell
            var newShell = App.Services.GetRequiredService<AppShell>();
            Application.Current.MainPage = newShell;
        }
    }
}
