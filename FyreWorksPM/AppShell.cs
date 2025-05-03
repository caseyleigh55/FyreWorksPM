using FyreWorksPM.Pages.Foundation;
using FyreWorksPM.Pages.Creation;
using FyreWorksPM.Services.Auth;

namespace FyreWorksPM;

/// <summary>
/// The main shell layout for authenticated users.
/// Dynamically builds tabs using DI-injected pages and provides a logout action.
/// </summary>
public partial class AppShell : Shell
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Constructs the AppShell and injects the authenticated page layout.
    /// </summary>
    public AppShell(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        BuildShellLayout();  // Inject and add Shell tabs
        RegisterRoutes();    // Register named routes for GoToAsync()
    }

    // ===============================
    // 🧱 Shell Layout Tabs (DI-based)
    // ===============================

    /// <summary>
    /// Builds the tabbed UI layout by resolving pages from DI.
    /// Adds each as a ShellContent entry.
    /// </summary>
    private void BuildShellLayout()
    {
        // 🏠 Home Page Tab
        var homePage = App.Services.GetRequiredService<HomePage>();
        Items.Add(new ShellContent
        {
            Title = "Home",
            Content = homePage,
            Route = "home"
        });

        // 🧾 Bids Page Tab
        var bidsPage = App.Services.GetRequiredService<BidsPage>();
        Items.Add(new ShellContent
        {
            Title = "Bids",
            Content = bidsPage,
            Route = "bids"
        });

        //=============================================================
        //
        // 🧪 TEMP: Expose CreateItemsPage directly for manual testing
        //
        //=============================================================
        var itemsPage = App.Services.GetRequiredService<CreateItemsPage>();
        Items.Add(new ShellContent
        {
            Title = "🔧 Manage Items (Temp)",
            Content = itemsPage,
            Route = "createitems" // still register route in case we navigate programmatically later
        });

        // 🛠 Projects Tab
        var projectsPage = App.Services.GetRequiredService<ProjectsPage>();
        Items.Add(new ShellContent
        {
            Title = "Projects",
            Content = projectsPage,
            Route = "projects"
        });

        // 🔧 Services Tab
        var servicePage = App.Services.GetRequiredService<ServicePage>();
        Items.Add(new ShellContent
        {
            Title = "Services",
            Content = servicePage,
            Route = "services"
        });

        // 🚪 Logout MenuItem (not a tab)
        Items.Add(new MenuItem
        {
            Text = "Logout",
            Command = new Command(async () =>
            {
                await _authService.LogoutAsync();

                // 🔄 Reset to LoginShell after logout
                Application.Current.MainPage = new LoginShell(_authService);
            })
        });
    }

    // ===============================
    // 🧭 Route Registration
    // ===============================

    /// <summary>
    /// Registers routes for Shell navigation (GoToAsync).
    /// Enables deep linking and non-tab routing.
    /// </summary>
    private void RegisterRoutes()
    {
        Routing.RegisterRoute("home", typeof(HomePage));
        Routing.RegisterRoute("bids", typeof(BidsPage));
        Routing.RegisterRoute("createitems", typeof(CreateItemsPage));
        Routing.RegisterRoute("projects", typeof(ProjectsPage));
        Routing.RegisterRoute("services", typeof(ServicePage));
        Routing.RegisterRoute("register", typeof(RegisterPage)); // Used by LoginShell
    }
}
