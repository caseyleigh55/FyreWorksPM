using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.ViewModels.Creation;

namespace FyreWorksPM.Pages.Creation;

/// <summary>
/// Page for creating a new client. Uses DI to inject the view model.
/// </summary>
public partial class CreateClientPage : ContentPage
{
    public Func<ClientModel, Task> ClientAddedCallback { get; set; }

    /// <summary>
    /// Constructor accepts DI-injected view model and sets it as the BindingContext.
    /// </summary>
    public CreateClientPage(CreateClientViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}