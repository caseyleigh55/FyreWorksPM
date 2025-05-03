using FyreWorksPM.Services.Auth;
using Microsoft.Maui.ApplicationModel;

namespace FyreWorksPM;

/// <summary>
/// The root application class for the MAUI app.
/// Responsible for restoring login state and launching the appropriate shell.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Exposes the global dependency injection container.
    /// </summary>
    public static IServiceProvider Services { get; private set; }

    private readonly IAuthService _authService;

    /// <summary>
    /// App constructor. Injected with DI container and authentication service.
    /// </summary>
    /// <param name="serviceProvider">The global service provider.</param>
    /// <param name="authService">The authentication service.</param>
    public App(IServiceProvider serviceProvider, IAuthService authService)
    {
        InitializeComponent();

        Services = serviceProvider;
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        // ===============================
        // 🔐 Restore login from secure storage
        // ===============================
        try
        {
            System.Diagnostics.Debug.WriteLine("[AUTH] TryRestoreLoginAsync started");

            // NOTE: Blocking on async here is safe — this is the app constructor and
            // no UI is live yet. We need to restore login *before* setting MainPage.
            _authService.TryRestoreLoginAsync().GetAwaiter().GetResult();

            System.Diagnostics.Debug.WriteLine("[AUTH] TryRestoreLoginAsync finished");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AUTH ERROR] {ex.Message}");
        }

        // ===============================
        // 🧭 Launch the correct shell
        // ===============================
        MainPage = _authService.IsLoggedIn
            ? Services.GetRequiredService<AppShell>()
            : Services.GetRequiredService<LoginShell>();
    }
}
