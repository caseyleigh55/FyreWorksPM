using FyreWorksPM.ViewModels.Foundation;
namespace FyreWorksPM.Pages.Foundation;

public partial class ServicePage : ContentPage
{
	public ServicePage(ServicePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}