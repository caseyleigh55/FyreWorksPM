using FyreWorksPM.ViewModels.Creation;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Pages.Editing;

namespace FyreWorksPM.Pages.Creation;

public partial class CreateTasksPage : ContentPage
{
    private int _suggestionIndex = -1;
    private Action<TaskDto>? _onTaskSelected;
    private readonly CreateTasksViewModel _viewModel;

    //===============================================\\
    //================= Constructor =================\\
    //===============================================\\
    public CreateTasksPage(CreateTasksViewModel vm, Action<TaskDto>? onTaskSelected = null)
	{
		InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
        _onTaskSelected = onTaskSelected;

        vm.RequestEditTaskPopup = async () =>
        {
            var selectedTask = vm.SelectedTask;
            if (selectedTask == null)
                return;
            var popup = new EditTaskPage(
                selectedTask,
                async () =>
                {
                    await vm.LoadTasksAsync();
                    vm.ApplyFilter();
                },
                vm.TaskService  // Or however you access the item service from the view model
            );
            await Shell.Current.Navigation.PushModalAsync(popup);
        };
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

}