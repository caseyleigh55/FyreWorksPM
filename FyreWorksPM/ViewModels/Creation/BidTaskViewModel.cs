using CommunityToolkit.Mvvm.ComponentModel;

namespace FyreWorksPM.ViewModels.Creation
{
    public partial class BidTaskViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private decimal cost;

        [ObservableProperty]
        private decimal sale;
    }
}
