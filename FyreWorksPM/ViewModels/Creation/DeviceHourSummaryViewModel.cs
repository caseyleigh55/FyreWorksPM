using CommunityToolkit.Mvvm.ComponentModel;

namespace FyreWorksPM.Pages.Creation
{
    public partial class DeviceHourSummaryViewModel : ObservableObject
    {
        [ObservableProperty] private string activityType;

        [ObservableProperty] private decimal normalCount;
        [ObservableProperty] private decimal liftCount;
        [ObservableProperty] private decimal panelCount;
        [ObservableProperty] private decimal pipeCount;

        [ObservableProperty] private decimal totalHours;
    }

}
