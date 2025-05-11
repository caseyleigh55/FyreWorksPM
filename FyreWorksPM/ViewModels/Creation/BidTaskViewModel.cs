using CommunityToolkit.Mvvm.ComponentModel;

namespace FyreWorksPM.ViewModels.Creation
{
    public partial class BidTaskViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private decimal cost;

        partial void OnCostChanged(decimal value)
        {
            System.Diagnostics.Debug.WriteLine($"Cost updated: {value}");
        }


        [ObservableProperty]
        private decimal sale;
    }
}
