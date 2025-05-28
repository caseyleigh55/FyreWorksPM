using CommunityToolkit.Mvvm.ComponentModel;

namespace FyreWorksPM.Pages.Creation
{
    public partial class DeviceHourSummaryViewModel : ObservableObject
    {
        [ObservableProperty] private string activityType;

        [ObservableProperty] private int normalCount;
        [ObservableProperty] private int liftCount;
        [ObservableProperty] private int panelCount;
        [ObservableProperty] private int pipeCount;

        [ObservableProperty] private double totalHours;
    }

}
