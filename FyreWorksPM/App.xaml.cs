using FyreWorksPM.Services.Auth;

namespace FyreWorksPM;

/// <summary>
/// Entry point for the application.
/// Uses DI to determine which shell to load based on login state.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Global access to DI container.
    /// Only use this for Shell-level resolution, not in ViewModels.
    /// </summary>
    public static IServiceProvider Services { get; private set; }

    private readonly IAuthService _authService;

    public App(IServiceProvider serviceProvider, IAuthService authService)
    {
        InitializeComponent();

        Services = serviceProvider;
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        // Decide which shell to use based on login state
        MainPage = _authService.IsLoggedIn
            ? Services.GetRequiredService<AppShell>()
            : Services.GetRequiredService<LoginShell>();
    }
}
