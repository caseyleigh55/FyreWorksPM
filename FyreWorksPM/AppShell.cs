using FyreWorksPM.Pages.Foundation;
using FyreWorksPM.Pages.Creation;
using FyreWorksPM.Services.Auth;
using FyreWorksPM.Pages.PopUps;

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
        Items.Add(new ShellContent
        {
            Title = "Home",
            ContentTemplate = new DataTemplate(() =>
                App.Services.GetRequiredService<HomePage>()),
            Route = "home"
        });

        // 🧾 Bids Page Tab
        Items.Add(new ShellContent
        {
            Title = "Bids",
            ContentTemplate = new DataTemplate(() =>
            App.Services.GetRequiredService<BidsPage>()),
            Route = "bids"
        });

        // 🧾 Create Bids Page Tab
        Items.Add(new ShellContent
        {
            Title = "CreateBid",
            ContentTemplate = new DataTemplate(() =>
                App.Services.GetRequiredService<CreateBidPage>()),
            Route = "createbid"
        });

        //🧪 TEMP: CreateClientPage Tab
        Items.Add(new ShellContent
        {
            Title = "CreateClient",
            ContentTemplate = new DataTemplate(() =>
                App.Services.GetRequiredService<CreateClientPage>()),
            Route = "createclient"
        });

        //🧪 TEMP: CreateTasksPage Tab
        Items.Add(new ShellContent
        {
            Title = "🔧 CreateTasks (Temp)",
            ContentTemplate = new DataTemplate(() =>
                App.Services.GetRequiredService<CreateTasksPage>()),
            Route = "createtasks"
        });

        // 🧪 TEMP: CreateItemsPage Tab
        Items.Add(new ShellContent
        {
            Title = "🔧 CreateItems (Temp)",
            ContentTemplate = new DataTemplate(() =>
                App.Services.GetRequiredService<CreateItemsPage>()),
            Route = "createitems"
        });

        // 🛠 Projects Tab
        Items.Add(new ShellContent
        {
            Title = "Projects",
            ContentTemplate = new DataTemplate(() =>
                App.Services.GetRequiredService<ProjectsPage>()),
            Route = "projects"
        });

        // 🔧 Services Tab
        Items.Add(new ShellContent
        {
            Title = "Services",
            ContentTemplate = new DataTemplate(() =>
                App.Services.GetRequiredService<ServicePage>()),
            Route = "services"
        });

        // 🚪 Logout MenuItem (not a tab)
        Items.Add(new MenuItem
        {
            Text = "Logout",
            Command = new Command(async () =>
            {
                await _authService.LogoutAsync();
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
        Routing.RegisterRoute("createclient", typeof(CreateClientPage));
        Routing.RegisterRoute("createtasks", typeof(CreateTasksPage));
        Routing.RegisterRoute("createBid", typeof(CreateBidPage));
        Routing.RegisterRoute("projects", typeof(ProjectsPage));
        Routing.RegisterRoute("services", typeof(ServicePage));
        Routing.RegisterRoute("register", typeof(CreateUserPage)); // Used by LoginShell
    }
}
