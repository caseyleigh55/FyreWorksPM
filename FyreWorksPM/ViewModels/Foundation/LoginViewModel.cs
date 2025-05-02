using System.Windows.Input;
using FyreWorksPM.Services.Auth;
using FyreWorksPM.Services.Navigation;

namespace FyreWorksPM.ViewModels.Foundation;

public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _auth;
    private readonly INavigationService _nav;

    public string Username { get => Get<string>(); set => Set(value); }
    public string Password { get => Get<string>(); set => Set(value); }

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    public LoginViewModel(IAuthService auth, INavigationService nav)
    {
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        _nav = nav ?? throw new ArgumentNullException(nameof(nav));

        LoginCommand = new Command(async () => await LoginAsync());
        GoToRegisterCommand = new Command(async () => await _nav.GoToAsync("//register"));
    }

    public async Task<bool> LoginAsync()
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

        // Optionally: Navigate to main app shell or home page on success
        await _nav.GoToAsync("//home"); // or "//dashboard", etc.

        return true;
    }
}
