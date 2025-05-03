using FyreWorksPM.Services.Auth;
using Microsoft.Maui.ApplicationModel;

namespace FyreWorksPM;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    private readonly IAuthService _authService;

    public App(IServiceProvider serviceProvider, IAuthService authService)
    {
        InitializeComponent();

        Services = serviceProvider;
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        // ✅ Block until login restore finishes BEFORE setting MainPage
        try
        {
            System.Diagnostics.Debug.WriteLine("[AUTH] TryRestoreLoginAsync started");
            _authService.TryRestoreLoginAsync().GetAwaiter().GetResult();
            System.Diagnostics.Debug.WriteLine("[AUTH] TryRestoreLoginAsync finished");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AUTH ERROR] {ex.Message}");
        }

        MainPage = _authService.IsLoggedIn
            ? Services.GetRequiredService<AppShell>()
            : Services.GetRequiredService<LoginShell>();
    }
}





