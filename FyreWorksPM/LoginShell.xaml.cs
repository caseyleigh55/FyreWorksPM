using FyreWorksPM;
using FyreWorksPM.Pages.Foundation;
using FyreWorksPM.Pages.Creation;
using FyreWorksPM.Services.Auth;

namespace FyreWorksPM;

/// <summary>
/// Shell used for unauthenticated users (Login and Register only).
/// This layout is created programmatically and listens for auth state changes.
/// </summary>
public partial class LoginShell : Shell
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Constructs the LoginShell and listens for auth changes.
    /// </summary>
    /// <param name="authService">Injected authentication service.</param>
    public LoginShell(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        InitializeShell();
        _authService.AuthStateChanged += OnAuthStateChanged;
    }

    // ===============================
    // 🧱 Shell Layout Setup
    // ===============================

    /// <summary>
    /// Builds the login shell layout with Login and Register tabs.
    /// Pages are resolved using DI.
    /// </summary>
    private void InitializeShell()
    {
        // ✅ Register navigation routes (clean GoToAsync support)
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));

        // Resolve pages using DI
        var loginPage = App.Services.GetRequiredService<LoginPage>();
        var registerPage = App.Services.GetRequiredService<RegisterPage>();

        // 🧑‍💻 Login Tab
        Items.Add(new ShellContent
        {
            Title = "Login",
            Content = loginPage,
            Route = "login"
        });

        // 🆕 Register Tab
        Items.Add(new ShellContent
        {
            Title = "Register",
            Content = registerPage,
            Route = "register"
        });
    }

    // ===============================
    // 🔄 Auth Event Listener
    // ===============================

    /// <summary>
    /// Handles login state change.
    /// Swaps to AppShell when user successfully logs in.
    /// </summary>
    private void OnAuthStateChanged(object sender, AuthStateChangedEventArgs e)
    {
        if (e.IsLoggedIn)
        {
            // 🔁 Replace shell with authenticated layout
            var newShell = App.Services.GetRequiredService<AppShell>();
            Application.Current.MainPage = newShell;
        }
    }
}
