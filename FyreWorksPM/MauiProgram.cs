﻿using CommunityToolkit.Maui;
using FyreWorksPM.Configuration;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.Pages.Creation;
using FyreWorksPM.Pages.Editing;
using FyreWorksPM.Pages.Foundation;
using FyreWorksPM.Pages.PopUps;
using FyreWorksPM.Pages.Solitary;
using FyreWorksPM.Services.Auth;
using FyreWorksPM.Services.Bid;
using FyreWorksPM.Services.Client;
using FyreWorksPM.Services.Common;
using FyreWorksPM.Services.Item;
using FyreWorksPM.Services.Labor;
using FyreWorksPM.Services.Navigation;
using FyreWorksPM.Services.Tasks;
using FyreWorksPM.ViewModels.Creation;
using FyreWorksPM.ViewModels.Editing;
using FyreWorksPM.ViewModels.Foundation;
using FyreWorksPM.ViewModels.Solitary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

        builder.Services.AddHttpClient<ILaborTemplateService, LaborTemplateService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl); // Make sure BaseUrl ends with a `/`
        });



        // ============================
        // 🔧 Core Services
        // ============================
        //builder.Services.AddDbContext<ApplicationDbContext>(options =>
        //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<INavigationServices, NavigationService>();
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
        builder.Services.AddTransient<CreateUserViewModel>();
        builder.Services.AddTransient<CreateTasksViewModel>();

        // Pop-Up ViewModels
        builder.Services.AddTransient<EditItemViewModel>();
        builder.Services.AddTransient<EditItemTypeViewModel>();
        builder.Services.AddTransient<EditLaborTemplateViewModel>();

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
        builder.Services.AddTransient<CreateUserPage>();

        // Pop-Up Pages
        builder.Services.AddTransient<EditItemPage>();
        builder.Services.AddTransient<EditItemTypePage>();
        builder.Services.AddTransient<EditLaborTemplatePage>();

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