using System;
using System.Threading.Tasks;

namespace FyreWorksPM.Services.Navigation;

/// <summary>
/// Centralized navigation helper that abstracts away Shell and Navigation APIs.
/// Allows ViewModels to perform navigation cleanly without knowing about the UI layer.
/// </summary>
public class NavigationService : INavigationServices
{
    private readonly IServiceProvider _services;

    /// <summary>
    /// Constructs a new NavigationService using dependency injection.
    /// </summary>
    /// <param name="services">The DI container used to resolve pages.</param>
    public NavigationService(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Navigates to a Shell route (e.g. "login", "//home").
    /// </summary>
    /// <param name="route">The route to navigate to. Use "//" for absolute routes.</param>
    /// <param name="animate">Whether to animate the transition.</param>
    public async Task GoToAsync(string route, bool animate = true)
    {
        await Shell.Current.GoToAsync(route, animate);
    }

    /// <summary>
    /// Pushes a page onto the navigation stack using dependency injection.
    /// Great for modal pages or when not using Shell routing.
    /// </summary>
    /// <typeparam name="TPage">The type of page to push (must be registered in DI).</typeparam>
    public async Task PushPageAsync<TPage>() where TPage : Page
    {
        //var page = _services.GetRequiredService<TPage>();
        //await Shell.Current.Navigation.PushAsync(page);
        var page = _services.GetRequiredService<TPage>();

        if (page is IHideFlyout)
        {
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
        }
        else
        {
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        }

        await Shell.Current.Navigation.PushAsync(page);
    }

    /// <summary>
    /// Pops the current page off the navigation stack.
    /// </summary>
    public async Task PopAsync()
    {
        await Shell.Current.Navigation.PopAsync();
    }

    /// <summary>
    /// Navigates to the home page route (e.g. "//home").
    /// Use this to reset the stack after login or logout.
    /// </summary>
    public async Task GoHomeAsync()
    {
        await Shell.Current.GoToAsync("//home");
    }

    // 🚀 Add more helpers if needed, like GoBackAsync, GoToWithParams, etc.
}
