using FyreWorksPM.ViewModels.Creation;

namespace FyreWorksPM.Pages.Creation;

/// <summary>
/// Placeholder registration page.
/// </summary>
public partial class CreateUserPage : ContentPage
{
    public CreateUserPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
