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

        // ============================
        // 🔧 Core Services
        // ============================
        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ILoadingService, LoadingService>();
        builder.Services.AddSingleton<IItemService, ItemService>();
        builder.Services.AddSingleton<IItemTypeService, ItemTypeService>();

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
        builder.Services.AddSingleton<CreateBidPage>();
        builder.Services.AddSingleton<CreateClientPage>();
        builder.Services.AddSingleton<CreateItemsPage>();
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
        builder.Services.AddSingleton<CreateBidViewModel>();
        builder.Services.AddSingleton<CreateClientViewModel>();
        builder.Services.AddSingleton<CreateItemsViewModel>();

        // Pop-Up ViewModels
        builder.Services.AddSingleton<ManageItemPopupViewModel>();
        builder.Services.AddSingleton<ManageItemTypesPopupViewModel>();

        // ============================
        // 🏁 Build & Return
        // ============================
        var app = builder.Build();
        return app;
    }
}
