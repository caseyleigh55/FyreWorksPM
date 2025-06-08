using CommunityToolkit.Mvvm.ComponentModel;

namespace FyreWorksPM.ViewModels.Creation
{  

    public partial class InstallLocationHoursViewModel : ObservableObject
    {
        [ObservableProperty]
        private string locationName;

        [ObservableProperty]
        private decimal normalHours;

        [ObservableProperty]
        private decimal liftHours;

        [ObservableProperty]
        private decimal panelHours;

        [ObservableProperty]
        private decimal pipeHours;
    }

}
