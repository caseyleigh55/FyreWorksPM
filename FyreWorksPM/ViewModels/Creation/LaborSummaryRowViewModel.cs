
using CommunityToolkit.Mvvm.ComponentModel;

namespace FyreWorksPM.ViewModels.Creation
{
    public partial class LaborSummaryRowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string location;

        [ObservableProperty]
        private decimal normalCount;

        [ObservableProperty]
        private decimal liftCount;

        [ObservableProperty]
        private decimal panelCount;

        [ObservableProperty]
        private decimal pipeCount;

        [ObservableProperty]
        private decimal totalHours;
    }


}
