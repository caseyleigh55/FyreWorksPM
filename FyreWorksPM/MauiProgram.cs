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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using FyreWorksPM.Configuration;

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
        builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        });



        // Register services
        //builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();      

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
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddSingleton<ProjectsPageViewModel>();
        builder.Services.AddSingleton<ServicePageViewModel>();

        //Pop Up ViewModels
        builder.Services.AddSingleton<ManageItemPopupViewModel>();
        builder.Services.AddSingleton<ManageItemTypesPopupViewModel>();
        // etc.


        var app = builder.Build();           // ✅ Build the app first
        return app;                         // ✅ Return the built app



    }

    
}