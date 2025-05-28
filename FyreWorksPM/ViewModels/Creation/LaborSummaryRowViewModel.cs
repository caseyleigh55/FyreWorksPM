
using CommunityToolkit.Mvvm.ComponentModel;

namespace FyreWorksPM.ViewModels.Creation
{
    public partial class LaborSummaryRowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string location;

        [ObservableProperty]
        private int normalCount;

        [ObservableProperty]
        private int liftCount;

        [ObservableProperty]
        private int panelCount;

        [ObservableProperty]
        private int pipeCount;

        [ObservableProperty]
        private double totalHours;
    }


}
