using FyreWorksPM.ViewModels.Creation;
using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Pages.Creation;

public partial class CreateTasksPage : ContentPage
{
	public CreateTasksPage(CreateTasksViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}