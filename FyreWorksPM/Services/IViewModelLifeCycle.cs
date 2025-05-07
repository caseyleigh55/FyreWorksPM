public interface IViewModelLifecycle
{
    Task OnAppearingAsync();
    Task OnDisappearingAsync();
}
