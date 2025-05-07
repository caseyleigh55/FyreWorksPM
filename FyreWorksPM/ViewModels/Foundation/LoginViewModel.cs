using System.Windows.Input;
using FyreWorksPM.Services.Auth;
using FyreWorksPM.Services.Common;
using FyreWorksPM.Services.Navigation;

namespace FyreWorksPM.ViewModels.Foundation;

/// <summary>
/// ViewModel responsible for handling user login logic.
/// Connects the login UI to AuthService and controls Shell navigation.
/// </summary>
public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _auth;
    private readonly INavigationService _nav;
    private readonly ILoadingService _loading;


    // =====================
    // 🔗 Bindable Properties
    // =====================

    /// <summary>
    /// Username entered by the user.
    /// </summary>
    public string Username { get => Get<string>(); set => Set(value); }

    /// <summary>
    /// Password entered by the user.
    /// </summary>
    public string Password { get => Get<string>(); set => Set(value); }

    // =====================
    // 🎮 Commands
    // =====================

    /// <summary>
    /// Command triggered when the Login button is clicked.
    /// </summary>
    public ICommand LoginCommand { get; }

    /// <summary>
    /// Command triggered when the user opts to navigate to the registration screen.
    /// </summary>
    public ICommand GoToRegisterCommand { get; }

    // =====================
    // 🔨 Constructor
    // =====================

    /// <summary>
    /// Constructs the LoginViewModel with required auth and navigation services.
    /// </summary>
    public LoginViewModel(IAuthService auth, INavigationService nav, ILoadingService loading)
    {
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        _nav = nav ?? throw new ArgumentNullException(nameof(nav));
        _loading = loading ?? throw new ArgumentNullException(nameof(loading));

        LoginCommand = new Command(async () => await LoginAsync());
        GoToRegisterCommand = new Command(async () => await _nav.GoToAsync("//register"));
    }

    // =====================
    // 🧼 Public Helpers
    // =====================

    /// <summary>
    /// Clears the form fields, useful on logout or reset.
    /// </summary>
    public void Reset()
    {
        Username = string.Empty;
        Password = string.Empty;
    }

    // =====================
    // 🔐 Login Logic
    // =====================

    /// <summary>
    /// Validates input, calls AuthService, and switches to the authenticated AppShell if successful.
    /// </summary>
    /// <returns>True if login succeeded, false otherwise.</returns>
    public async Task<bool> LoginAsync()
    {
        await _loading.ShowAsync();
        try
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await App.Current.MainPage.DisplayAlert("Login Failed", "Enter username and password.", "OK");
                return false;
            }

            bool success = await _auth.LoginAsync(Username, Password);
            if (!success)
            {
                await App.Current.MainPage.DisplayAlert("Login Failed", "Invalid credentials.", "OK");
                return false;
            }

            // ✅ Successful login — switch to authenticated shell
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Application.Current.MainPage = App.Services.GetRequiredService<AppShell>();
            });

            // 👇 Force shell to go to the home tab
            await Shell.Current.GoToAsync("//home");

            return true;
        }
        finally
        {
            await _loading.HideAsync();
        }
    }
}
