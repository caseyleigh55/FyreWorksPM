using CommunityToolkit.Mvvm.ComponentModel;

namespace FyreWorksPM.ViewModels.Creation
{  

    public partial class InstallLocationHoursViewModel : ObservableObject
    {
        [ObservableProperty]
        private string locationName;

        [ObservableProperty]
        private double normalHours;

        [ObservableProperty]
        private double liftHours;

        [ObservableProperty]
        private double panelHours;

        [ObservableProperty]
        private double pipeHours;
    }

}
