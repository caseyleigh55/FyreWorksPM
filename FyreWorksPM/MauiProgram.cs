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
using FyreWorksPM.Services.Common;
using FyreWorksPM.Configuration;
using FyreWorksPM.Services.Item;
using FyreWorksPM.Services.Client;

namespace FyreWorksPM;

/// <summary>
/// The entry point for app setup and DI registration.
/// Configures all pages, services, viewmodels, and libraries.
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // ============================
        // 🔧 Core App + Fonts
        // ============================
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
            })
            .UseMauiCommunityToolkit();

        // ============================
        // 🌐 HTTP Clients
        // ============================
        builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        });

        builder.Services.AddHttpClient<RegisterViewModel>(); // ⚠️ Used to inject HttpClient into RegisterViewModel

        builder.Services.AddHttpClient<IItemTypeService, ItemTypeService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        });
    
        builder.Services.AddHttpClient<IItemService, ItemService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        });

        builder.Services.AddHttpClient<IClientService, ClientService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        });


        // ============================
        // 🔧 Core Services
        // ============================
        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ILoadingService, LoadingService>();        

        // ============================
        // 🐚 Shells
        // ============================
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<LoginShell>();

        // ============================
        // 📄 Pages
        // ============================

        // Foundation Pages
        builder.Services.AddSingleton<BidsPage>();
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<ProjectsPage>();
        builder.Services.AddSingleton<ServicePage>();

        // Creation Pages
        builder.Services.AddTransient<CreateBidPage>();
        builder.Services.AddTransient<CreateClientPage>();
        builder.Services.AddTransient<CreateItemsPage>();
        builder.Services.AddTransient<RegisterPage>();

        // Pop-Up Pages
        builder.Services.AddSingleton<ManageItemPopup>();
        builder.Services.AddSingleton<ManageItemTypesPopup>();

        // Solitary Pages
        builder.Services.AddSingleton<SelectedBidPage>();
        builder.Services.AddSingleton<SelectedProjectPage>();
        builder.Services.AddSingleton<SelectedTicketPage>();

        // ============================
        // 🧠 ViewModels
        // ============================

        // Foundation ViewModels
        builder.Services.AddSingleton<AppShellViewModel>();
        builder.Services.AddSingleton<BidsPageViewModel>();
        builder.Services.AddSingleton<HomePageViewModel>();
        builder.Services.AddSingleton<LoginShellViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddSingleton<ProjectsPageViewModel>();
        builder.Services.AddSingleton<ServicePageViewModel>();

        // Creation ViewModels
        builder.Services.AddTransient<CreateBidViewModel>();
        builder.Services.AddTransient<CreateClientViewModel>();
        builder.Services.AddTransient<CreateItemsViewModel>();

        // Pop-Up ViewModels
        builder.Services.AddTransient<ManageItemPopupViewModel>();
        builder.Services.AddTransient<ManageItemTypesPopupViewModel>();

        // ============================
        // 🏁 Build & Return
        // ============================
        var app = builder.Build();
        return app;
    }
}
