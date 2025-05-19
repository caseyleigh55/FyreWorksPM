using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Services.Client;
using FyreWorksPM.Services.Tasks;
using FyreWorksPM.ViewModels.Editing;
namespace FyreWorksPM.Pages.Editing;

public partial class EditTaskPage : ContentPage
{
    private bool isClosingInternally = false;

    //===============================================\\
    //================= Constructor =================\\
    //===============================================\\
    public EditTaskPage(TaskDto task, Func<Task> onSaved, ITaskService service)
	{
		InitializeComponent();
        BindingContext = new EditTaskViewModel(task, onSaved, service);
    }

    public void MarkClosingInternally()
    {
        isClosingInternally = true;
    }

    /// <summary>
    /// Handles the cancel button click by closing the popup view.
    /// </summary>
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}