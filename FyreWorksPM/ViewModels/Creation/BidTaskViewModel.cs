using CommunityToolkit.Mvvm.ComponentModel;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Enums;
using System.Diagnostics;

namespace FyreWorksPM.ViewModels.Creation
{
    public partial class BidTaskViewModel : ObservableObject
    {
        [ObservableProperty]
        private int taskModelId;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private decimal cost;

        [ObservableProperty]
        private decimal sale;

        [ObservableProperty]
        private TaskType type;

        [ObservableProperty]
        private string selectedTemplateName;

        [ObservableProperty]
        private TaskDto selectedTemplateTaskName;

        partial void OnSelectedTemplateTaskNameChanged(TaskDto value)
        {
            Debug.WriteLine($"🔥 Template selected: {value?.TaskName}");
            if (value == null) return;

            // Set the task name and prices based on the template
            Name = value.TaskName;
            Cost = value.DefaultCost;
            Sale = value.DefaultSale;
            TaskModelId = value.Id;

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Cost));
            OnPropertyChanged(nameof(Sale));

        }

    }
}
