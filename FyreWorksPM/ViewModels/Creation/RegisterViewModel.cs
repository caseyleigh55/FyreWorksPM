using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.Services.Navigation;

namespace FyreWorksPM.ViewModels.Creation;

/// <summary>
/// ViewModel responsible for user registration logic.
/// Handles form validation, password hashing, and saving new users to the database.
/// </summary>
public class RegisterViewModel : ViewModelBase
{
    private readonly ApplicationDbContext _db;
    private readonly INavigationService _navigation;

    // Bindable properties for form fields
    public string Username { get => Get<string>(); set => Set(value); }
    public string Email { get => Get<string>(); set => Set(value); }
    public string Password { get => Get<string>(); set => Set(value); }
    public string ConfirmPassword { get => Get<string>(); set => Set(value); }

    // Command bound to the registration button
    public ICommand RegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }

    /// <summary>
    /// Constructor for RegisterViewModel.
    /// Injects DB context and navigation service via DI.
    /// </summary>
    public RegisterViewModel(INavigationService navigation)
    {
       
        _navigation = navigation;

        RegisterCommand = new Command(async () => await RegisterAsync());
        GoToLoginCommand = new Command(async () => await ReturnToLogin());

    }

    private async Task ReturnToLogin()
    {
        await _navigation.GoToAsync("//login");
    }

    /// <summary>
    /// Validates form input and registers a new user securely.
    /// </summary>
    private async Task RegisterAsync()
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage
               .DisplayAlert("Error", "All fields are required", "OK");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await Application.Current.MainPage
               .DisplayAlert("Error", "Passwords do not match", "OK");
            return;
        }

        // Check if user already exists by username or email
        if (await _db.Users.AnyAsync(u =>
            u.Username == Username.Trim() ||
            u.Email == Email.Trim()))
        {
            await Application.Current.MainPage
               .DisplayAlert("Error", "User with this username or email already exists.", "OK");
            return;
        }

        // Securely hash the password using BCrypt (with salt baked in)
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);

        var user = new UserModel
        {
            Username = Username.Trim(),
            Email = Email.Trim(),
            PasswordHash = passwordHash
        };

        // Save the user to the database
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await Application.Current.MainPage
           .DisplayAlert("Success", "User created!", "OK");

        // 🔄 Navigate to login page using the navigation service
        await _navigation.GoToAsync("login");
    }

    
}
