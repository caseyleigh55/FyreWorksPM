using System.Windows.Input;
using FyreWorksPM.Services.Auth;

namespace FyreWorksPM.ViewModels.Foundation
{
    /// <summary>
    /// ViewModel for the main/home page.
    /// Handles logout functionality and main app transitions.
    /// </summary>
    public class HomePageViewModel : ViewModelBase
    {
        private readonly IAuthService _auth;

        #region Commands

        /// <summary>
        /// Command to log the user out and return to the login screen.
        /// </summary>
        public ICommand LogOutCommand { get; }

        #endregion

        /// <summary>
        /// Constructor for MainPageViewModel.
        /// </summary>
        /// <param name="auth">Authentication service injected via DI.</param>
        public HomePageViewModel(IAuthService auth)
        {
            _auth = auth;

            // Assign logout logic to command
            LogOutCommand = new Command(async () => await _auth.LogoutAsync());
        }
    }
}
