using System.Windows.Input;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.Services.Auth;
using FyreWorksPM.Services.Navigation;
using Microsoft.EntityFrameworkCore;

namespace FyreWorksPM.ViewModels.Foundation;

public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _auth;
    private readonly INavigationService _nav; // 👈 Injected service
    private readonly ApplicationDbContext _db;

    public string Username { get => Get<string>(); set => Set(value); }
    public string Password { get => Get<string>(); set => Set(value); }

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    public LoginViewModel(IAuthService auth, INavigationService nav,ApplicationDbContext db)
    {
        _db = db;
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        _nav = nav ?? throw new ArgumentNullException(nameof(nav));

        LoginCommand = new Command(async () => await LoginAsync());
        GoToRegisterCommand = new Command(async () => await _nav.GoToAsync("//register")); // 👈 Clean!
    }

    public async Task<bool> LoginAsync()
    {
        var dbPath = _db?.Database.GetDbConnection()?.ConnectionString;
        System.Diagnostics.Debug.WriteLine($"[AUTH] DB Connection = {dbPath}");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == Username);

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
         
        return true;
    }
}
