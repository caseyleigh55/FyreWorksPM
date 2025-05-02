using FyreWorksPM.Services.Auth;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FyreWorksPM.ViewModels.Foundation;

public partial class AppShellViewModel : ObservableObject
{
    private readonly IAuthService _auth;

    [ObservableProperty]
    private bool isLoggedIn;

    public bool IsLoggedOut => !isLoggedIn;

    public AppShellViewModel(IAuthService auth)
    {
        _auth = auth;

        isLoggedIn = _auth.IsLoggedIn;

        _auth.AuthStateChanged += (s, e) =>
        {
            isLoggedIn = _auth.IsLoggedIn;
            OnPropertyChanged(nameof(IsLoggedOut));
        };
    }
}
