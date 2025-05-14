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
using FyreWorksPM.Services.Bid;
using FyreWorksPM.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FyreWorksPM.Services.Tasks;

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

        builder.Services.AddHttpClient(); // ⚠️ default HttpClient Instance

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

        builder.Services.AddHttpClient<IBidService, BidService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        });

        builder.Services.AddHttpClient<ITaskService, TaskService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        });


        // ============================
        // 🔧 Core Services
        // ============================
        //builder.Services.AddDbContext<ApplicationDbContext>(options =>
        //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ILoadingService, LoadingService>();


        // ============================
        // 🐚 Shells
        // ============================
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<LoginShell>();

        // ============================
        // 🧠 ViewModels
        // ============================

        // Foundation ViewModels
        builder.Services.AddTransient<AppShellViewModel>();
        builder.Services.AddTransient<BidsPageViewModel>();
        builder.Services.AddTransient<HomePageViewModel>();
        builder.Services.AddTransient<LoginShellViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ProjectsPageViewModel>();
        builder.Services.AddTransient<ServicePageViewModel>();

        // Creation ViewModels
        builder.Services.AddTransient<CreateBidViewModel>();
        builder.Services.AddTransient<CreateClientViewModel>();
        builder.Services.AddTransient<CreateItemsViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<CreateTasksViewModel>();

        // Pop-Up ViewModels
        builder.Services.AddTransient<ManageItemPopupViewModel>();
        builder.Services.AddTransient<ManageItemTypesPopupViewModel>();

        // ============================
        // 📄 Pages (DI-resolved with ViewModels where needed)
        // ============================

        // Foundation Pages
        builder.Services.AddTransient<BidsPage>(provider =>
        {
            var vm = provider.GetRequiredService<BidsPageViewModel>();
            return new BidsPage(vm);
        });
        builder.Services.AddTransient<HomePage>();
        
        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddTransient<ProjectsPage>(provider =>
        {
            var vm = provider.GetRequiredService<ProjectsPageViewModel>();
            return new ProjectsPage(vm);
        });
        builder.Services.AddTransient<ServicePage>(provider =>
        {
            var vm = provider.GetRequiredService<ServicePageViewModel>();
            return new ServicePage(vm);
        });

        // Creation Pages
        builder.Services.AddTransient<CreateTasksPage>();

        builder.Services.AddTransient<CreateBidPage>(provider =>
        {
            var vm = provider.GetRequiredService<CreateBidViewModel>();
            var clientPage = provider.GetRequiredService<CreateClientPage>();
            return new CreateBidPage(vm, clientPage);
        });

        builder.Services.AddTransient<CreateClientPage>(provider =>
        {
            var vm = provider.GetRequiredService<CreateClientViewModel>();
            return new CreateClientPage(vm);
        });
        builder.Services.AddTransient<CreateItemsPage>(provider =>
        {
            var vm = provider.GetRequiredService<CreateItemsViewModel>();
            return new CreateItemsPage(vm);
        });
        builder.Services.AddTransient<RegisterPage>();

        // Pop-Up Pages
        builder.Services.AddTransient<ManageItemPopup>();

        builder.Services.AddTransient<ManageItemTypesPopup>();

        // Solitary Pages
        builder.Services.AddTransient<SelectedBidPage>();
        builder.Services.AddTransient<SelectedProjectPage>();
        builder.Services.AddTransient<SelectedTicketPage>();

        

        // ============================
        // 🏁 Build & Return
        // ============================
        var app = builder.Build();
        return app;
    }
}