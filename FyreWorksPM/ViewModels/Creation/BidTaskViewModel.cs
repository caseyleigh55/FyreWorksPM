using CommunityToolkit.Mvvm.ComponentModel;
using FyreWorksPM.DataAccess.Enums;

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

    }
}
