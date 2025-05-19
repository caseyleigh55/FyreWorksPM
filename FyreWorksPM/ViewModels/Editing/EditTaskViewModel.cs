using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Enums;
using FyreWorksPM.Pages.Editing;
using FyreWorksPM.Services.Tasks;
using System.Collections.ObjectModel;


namespace FyreWorksPM.ViewModels.Editing
{
    public partial class EditTaskViewModel : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string taskName;

        [ObservableProperty]
        private TaskType selectedTaskType;

        [ObservableProperty]
        private decimal defaultCost;

        [ObservableProperty]
        private decimal defaultSale;
        
        private readonly TaskDto _originalTask;
        private readonly Func<Task> _onSaved;
        private readonly ITaskService _taskService;

        public ObservableCollection<TaskType> TaskTypes { get; }


        //===============================================\\
        //================= Constructor =================\\
        //===============================================\\
        public EditTaskViewModel(TaskDto task, Func<Task> onSaved, ITaskService taskService)
        {
            _originalTask = task;
            _onSaved = onSaved;
            _taskService = taskService;
            TaskTypes = new ObservableCollection<TaskType>((TaskType[])Enum.GetValues(typeof(TaskType)));           
            
            taskName = task.TaskName;
            selectedTaskType = TaskType.Admin;
            defaultCost = task.DefaultCost;
            defaultSale = task.DefaultSale;
            _onSaved = onSaved;
            _taskService = taskService;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            var task = Shell.Current.CurrentPage as EditTaskPage;
            task?.MarkClosingInternally();
            _originalTask.TaskName = taskName;
            _originalTask.Type = selectedTaskType;
            _originalTask.DefaultCost = defaultCost;
            _originalTask.DefaultSale = defaultSale;            

            if (string.IsNullOrWhiteSpace(taskName))
            { 
                await Shell.Current.DisplayAlert("Validation Error", "Please fill out all fields.", "OK");
                return;
            }

            var dto = new TaskDto
            {
                TaskName = taskName,
                Type = selectedTaskType,
                DefaultCost = defaultCost,
                DefaultSale = defaultSale
            };

            await _taskService.UpdateTaskAsync(_originalTask.Id, dto);

            // Notify parent to refresh the list
            if (_onSaved != null)
                await _onSaved.Invoke();

            // Proceed with save logic
            await Shell.Current.GoToAsync("..");
        }
    }    
}
