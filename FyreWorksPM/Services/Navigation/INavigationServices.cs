namespace FyreWorksPM.Services.Navigation
{
    /// <summary>
    /// Provides an abstraction for navigation throughout the app.
    /// Decouples navigation logic from Views and ViewModels for testability and reuse.
    /// </summary>
    public interface INavigationServices
    {
        /// <summary>
        /// Navigates to a Shell route (e.g. "home", "//login").
        /// </summary>
        /// <param name="route">The route to navigate to (can be relative or absolute).</param>
        /// <param name="animate">Whether the transition should be animated.</param>
        Task GoToAsync(string route, bool animate = true);

        /// <summary>
        /// Pushes a page onto the navigation stack using dependency injection.
        /// </summary>
        /// <typeparam name="TPage">The page type to push.</typeparam>
        Task PushPageAsync<TPage>() where TPage : Page;

        /// <summary>
        /// Pops the current page off the navigation stack.
        /// </summary>
        Task PopAsync();

        /// <summary>
        /// Navigates directly to the home page route (e.g. "//home").
        /// </summary>
        Task GoHomeAsync();
    }
}
