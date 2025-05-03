using FyreWorksPM.Pages.Foundation;
using FyreWorksPM.Pages.Creation;
using FyreWorksPM.Services.Auth;

namespace FyreWorksPM;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;

    public AppShell(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        BuildShellLayout();  // 👈 All pages injected via DI
        RegisterRoutes();    // 👈 For manual GoToAsync() support
    }

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

        // 🛠 Projects Tab (optional)
        var projectsPage = App.Services.GetRequiredService<ProjectsPage>();
        Items.Add(new ShellContent
        {
            Title = "Projects",
            Content = projectsPage,
            Route = "projects"
        });

        // 🔧 Services Tab (optional)
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
                Application.Current.MainPage = new LoginShell(_authService);
            })
        });
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute("home", typeof(HomePage));
        Routing.RegisterRoute("bids", typeof(BidsPage));
        Routing.RegisterRoute("projects", typeof(ProjectsPage));
        Routing.RegisterRoute("services", typeof(ServicePage));
        Routing.RegisterRoute("register", typeof(RegisterPage)); // Optional if Register is deep-linked
    }
}
