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
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

}