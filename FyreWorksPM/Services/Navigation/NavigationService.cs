using System;
using System.Threading.Tasks;

namespace FyreWorksPM.Services.Navigation;

/// <summary>
/// Centralized navigation helper.
/// </summary>
public class NavigationService : INavigationService
{
    private IServiceProvider _services;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public async Task GoToAsync(string route, bool animate = true)
    {
        await Shell.Current.GoToAsync(route, animate);
    }

    public async Task PushPageAsync<TPage>() where TPage : Page
    {
        var page = _services.GetRequiredService<TPage>();
        await Shell.Current.Navigation.PushAsync(page);
    }

    public async Task PopAsync()
    {
        await Shell.Current.Navigation.PopAsync();
    }

    public async Task GoHomeAsync()
    {
        await Shell.Current.GoToAsync("//home");
    }

    // Add more helpers as needed!
}
