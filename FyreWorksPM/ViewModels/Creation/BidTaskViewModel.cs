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
        private SavedTaskDto selectedTemplateTaskName;

        partial void OnSelectedTemplateTaskNameChanged(SavedTaskDto value)
        {
            Debug.WriteLine($"🔥 Template selected: {value?.SavedTaskDtoTaskName}");
            if (value == null) return;

            // Set the task name and prices based on the template
            Name = value.SavedTaskDtoTaskName;
            Cost = value.SavedTaskDtoDefaultCost;
            Sale = value.SavedTaskDtoDefaultSale;
            TaskModelId = value.SavedTaskDtoId;

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Cost));
            OnPropertyChanged(nameof(Sale));

        }

    }
}
