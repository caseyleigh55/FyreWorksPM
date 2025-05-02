using FyreWorksPM.ViewModels.Foundation;
namespace FyreWorksPM.Pages.Foundation;

public partial class ProjectsPage : ContentPage
{
	public ProjectsPage(ProjectsPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}