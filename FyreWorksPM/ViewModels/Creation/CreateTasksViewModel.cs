using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Enums;
using FyreWorksPM.Services.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FyreWorksPM.ViewModels.Creation
{
    public partial class CreateTasksViewModel : ObservableObject
    {
        private readonly ITaskService _taskService;

        public CreateTasksViewModel(ITaskService taskService)
        {
            _taskService = taskService;

            TaskTypes = new ObservableCollection<TaskType>((TaskType[])Enum.GetValues(typeof(TaskType)));
            SelectedTaskType = TaskType.Admin;

            LoadTasksCommand.Execute(null);
        }

        // ----------------------------
        // Input Fields
        // ----------------------------
        [ObservableProperty] private string taskName;
        [ObservableProperty] private decimal defaultCost;
        [ObservableProperty] private decimal defaultSale;
        [ObservableProperty] private TaskType selectedTaskType;

        // ----------------------------
        // Collections
        // ----------------------------
        public ObservableCollection<TaskType> TaskTypes { get; }

        public ObservableCollection<SavedTaskDto> AllTasks { get; } = new();

        [ObservableProperty]
        private ObservableCollection<SavedTaskDto> filteredTasks = new();

        // ----------------------------
        // UI-Related State
        // ----------------------------
        [ObservableProperty] private SavedTaskDto selectedTask;
        [ObservableProperty] private string searchText;

        // ----------------------------
        // Load tasks from API
        // ----------------------------
        [RelayCommand]
        private async Task LoadTasksAsync()
        {
            Debug.WriteLine($"Loading tasks for: {SelectedTaskType}");

            var tasks = await _taskService.GetTemplatesByTypeAsync(SelectedTaskType);
            AllTasks.Clear();

            foreach (var task in tasks)
            {
                Debug.WriteLine($"Loaded Task: {task.SavedTaskDtoTaskName} - {task.SavedTaskDtoType}");
                AllTasks.Add(task);
            }

            ApplyFilter();
        }

        // ----------------------------
        // Filtering
        // ----------------------------
        partial void OnSearchTextChanged(string value) => ApplyFilter();
        partial void OnSelectedTaskTypeChanged(TaskType value) => LoadTasksCommand.Execute(null);

        private void ApplyFilter()
        {
            var query = SearchText?.ToLowerInvariant() ?? "";

            var filtered = AllTasks
                .Where(t => string.IsNullOrWhiteSpace(query) ||
                            t.SavedTaskDtoTaskName.ToLowerInvariant().Contains(query))
                .ToList();

            filteredTasks.Clear();
            foreach (var task in filtered)
                filteredTasks.Add(task);
        }

        // ----------------------------
        // Add new task
        // ----------------------------
        [RelayCommand]
        private async Task AddTaskAsync()
        {
            if (string.IsNullOrWhiteSpace(TaskName))
                return;

            var dto = new CreateTaskDto
            {
                CreateTaskDtoTaskName = TaskName,
                CreateTaskDtoType = SelectedTaskType,
                CreateTaskDtoDefaultCost = DefaultCost,
                CreateTaskDtoDefaultSale = DefaultSale
            };

            var result = await _taskService.CreateTemplateAsync(dto);
            if (result is not null)
            {
                AllTasks.Add(result);
                ApplyFilter();
                ClearForm();
            }
        }

        // ----------------------------
        // Delete selected task
        // ----------------------------
        [RelayCommand]
        private async Task DeleteSelectedTaskAsync()
        {
            if (SelectedTask is null)
                return;

            await _taskService.DeleteTemplateAsync(SelectedTask.SavedTaskDtoId);
            AllTasks.Remove(SelectedTask);
            filteredTasks.Remove(SelectedTask);
            SelectedTask = null;
        }

        // ----------------------------
        // Edit selected task
        // ----------------------------
        [RelayCommand]
        private async Task EditSelectedTaskAsync()
        {
            if (SelectedTask is null)
                return;

            var dto = new CreateTaskDto
            {
                CreateTaskDtoTaskName = SelectedTask.SavedTaskDtoTaskName,
                CreateTaskDtoType = SelectedTask.SavedTaskDtoType,
                CreateTaskDtoDefaultCost = SelectedTask.SavedTaskDtoDefaultCost,
                CreateTaskDtoDefaultSale = SelectedTask.SavedTaskDtoDefaultSale
            };

            await _taskService.UpdateTemplateAsync(SelectedTask.SavedTaskDtoId, dto);
            await LoadTasksAsync(); // refresh the list
        }

        private void ClearForm()
        {
            TaskName = string.Empty;
            DefaultCost = 0;
            DefaultSale = 0;
        }
    }
}
