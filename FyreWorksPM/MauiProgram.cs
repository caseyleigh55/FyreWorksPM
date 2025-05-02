using FyreWorksPM.Services.Auth;
using CommunityToolkit.Maui;
using FyreWorksPM.Pages.Creation;
using FyreWorksPM.Pages.Foundation;
using FyreWorksPM.ViewModels.Creation;
using FyreWorksPM.ViewModels.Foundation;
using FyreWorksPM.Pages.PopUps;
using FyreWorksPM.Pages.Solitary;
using FyreWorksPM.ViewModels.Solitary;
using FyreWorksPM.Services.Navigation;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FyreWorksPM;
public static class MauiProgram
{
    
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
        }).UseMauiCommunityToolkit();

        //DB connection
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=FyreWorksPMDb;Trusted_Connection=True;"));


        // Register services
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddDbContext<ApplicationDbContext>();

        //Register Shells****************************
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<LoginShell>();

        // Register pages*****************************

        //Creation Pages
        builder.Services.AddSingleton<CreateBidPage>();
        builder.Services.AddSingleton<CreateClientPage>();
        builder.Services.AddSingleton<CreateItemsPage>();
        builder.Services.AddTransient<RegisterPage>();

        //Foundation Pages
        builder.Services.AddSingleton<BidsPage>();
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<ProjectsPage>();
        builder.Services.AddSingleton<ServicePage>();

        //Pop Up Pages
        builder.Services.AddSingleton<ManageItemPopup>();
        builder.Services.AddSingleton<ManageItemTypesPopup>();

        //Solitary Pages
        builder.Services.AddSingleton<SelectedBidPage>();
        builder.Services.AddSingleton<SelectedProjectPage>();
        builder.Services.AddSingleton<SelectedTicketPage>();

        //Register ViewModels

        //Creation ViewModels
        builder.Services.AddSingleton<CreateBidViewModel>();
        builder.Services.AddSingleton<CreateClientViewModel>();
        builder.Services.AddSingleton<CreateItemsViewModel>();
        builder.Services.AddSingleton<RegisterViewModel>();

        //Foundation ViewModels
        builder.Services.AddSingleton<AppShellViewModel>();
        builder.Services.AddSingleton<BidsPageViewModel>();                
        builder.Services.AddSingleton<HomePageViewModel>();
        builder.Services.AddSingleton<LoginShellViewModel>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<ProjectsPageViewModel>();
        builder.Services.AddSingleton<ServicePageViewModel>();

        //Pop Up ViewModels
        builder.Services.AddSingleton<ManageItemPopupViewModel>();
        builder.Services.AddSingleton<ManageItemTypesPopupViewModel>();
        // etc.


        var app = builder.Build();           // ✅ Build the app first
        SeedTestUser(app.Services);         // ✅ Now we can use the service provider
        return app;                         // ✅ Return the built app



    }

    private static void SeedTestUser(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Prevent dupes
        if (!db.Users.Any(u => u.Username == "admin"))
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
            db.Users.Add(new UserModel
            {
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = hashedPassword
            });
            db.SaveChanges();
        }
    }
}