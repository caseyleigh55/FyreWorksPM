using CommunityToolkit.Mvvm.ComponentModel;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Enums;
using System.Diagnostics;

namespace FyreWorksPM.ViewModels.Creation
{
    /// <summary>
    /// Represents a single bid task line item, bound to Admin or Engineering task sections.
    /// </summary>
    public partial class BidTaskViewModel : ObservableObject
    {
        #region 🔹 Template Selection

        /// <summary>
        /// The selected task template item.
        /// Triggers update of TaskModelId, Name, Cost, and Sale.
        /// </summary>
        [ObservableProperty]
        private TaskDto selectedTemplateTaskName;

        /// <summary>
        /// The selected task template's display name (used in search boxes).
        /// </summary>
        [ObservableProperty]
        private string selectedTemplateName;

        #endregion

        #region 🔹 Task Properties

        /// <summary>
        /// ID of the task template used (for DB tracking).
        /// </summary>
        [ObservableProperty]
        private int taskModelId;

        /// <summary>
        /// User-facing task name (e.g., "Drafting", "Submittals").
        /// </summary>
        [ObservableProperty]
        private string name;

        /// <summary>
        /// Internal cost for the task.
        /// </summary>
        [ObservableProperty]
        private decimal cost;

        /// <summary>
        /// External sale price for the task.
        /// </summary>
        [ObservableProperty]
        private decimal sale;

        /// <summary>
        /// The task type (Admin or Engineering).
        /// </summary>
        [ObservableProperty]
        private TaskType type;

        #endregion

        #region 🔹 Template Selection Logic

        /// <summary>
        /// When the user selects a task template, populate all task values from it.
        /// </summary>
        /// <param name="value">The selected template DTO.</param>
        partial void OnSelectedTemplateTaskNameChanged(TaskDto value)
        {
            Debug.WriteLine($"🔥 Template selected: {value?.TaskName}");
            if (value == null) return;

            Name = value.TaskName;
            Cost = value.DefaultCost;
            Sale = value.DefaultSale;
            TaskModelId = value.Id;

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Cost));
            OnPropertyChanged(nameof(Sale));
        }

        #endregion
    }
}
