namespace FyreWorksPM.Services.Navigation
{
    public interface INavigationService
    {
        Task GoToAsync(string route, bool animate = true);
        Task PushPageAsync<TPage>() where TPage : Page;
        Task PopAsync();
        Task GoHomeAsync();
    }
}
