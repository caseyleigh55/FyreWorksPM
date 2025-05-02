using Microsoft.Maui.Controls;
using FyreWorksPM.ViewModels.Foundation;
using FyreWorksPM.Services.Navigation; // 👈 New!
using FyreWorksPM.Pages.Creation;

namespace FyreWorksPM.Pages.Foundation;

/// <summary>
/// LoginPage handles user authentication and navigation to Register or AppShell.
/// </summary>
public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _vm;
    private readonly AppShell _appShell;
    private readonly INavigationService _nav; // 👈 Injected navigation service

    // 👇 Parameterless constructor for XAML preview/fallback
    public LoginPage() : this(
        App.Services.GetRequiredService<LoginViewModel>(),
        App.Services.GetRequiredService<AppShell>(),
        App.Services.GetRequiredService<NavigationService>())
    {
    }

    // 👇 Constructor used by DI
    public LoginPage(LoginViewModel vm, AppShell appShell, INavigationService nav)
    {
        InitializeComponent();

        _vm = vm ?? throw new ArgumentNullException(nameof(vm));
        _appShell = appShell ?? throw new ArgumentNullException(nameof(appShell));
        _nav = nav ?? throw new ArgumentNullException(nameof(nav));

        _vm.Reset();
        BindingContext = _vm;
    }

    /// <summary>
    /// Called when the Login button is clicked.
    /// Attempts login and swaps to the authenticated AppShell on success.
    /// </summary>
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        bool success = await _vm.LoginAsync();

        if (!success)
        {
            await DisplayAlert("Login Failed", "Invalid credentials", "OK");
            return;
        }

        // 🔄 Swap out shell to the main authenticated view
        Application.Current.MainPage = _appShell;

        await Task.Delay(50); // Let the UI settle
        await _nav.GoToAsync("//home"); // 👈 Clean navigation
    }

    /// <summary>
    /// Called when the Register button is clicked.
    /// Navigates to the Register page using DI.
    /// </summary>
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await _nav.PushPageAsync<RegisterPage>(); // 👈 No more "page already has parent" errors!
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.Reset(); // 💣 NOW it runs every time the page is shown, not just on creation
    }



}
