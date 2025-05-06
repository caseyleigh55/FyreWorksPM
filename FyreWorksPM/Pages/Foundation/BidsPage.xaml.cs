using FyreWorksPM.ViewModels.Foundation;

namespace FyreWorksPM.Pages.Foundation;

/// <summary>
/// Placeholder bids page (work in progress).
/// </summary>
public partial class BidsPage : ContentPage
{
    public BidsPage(BidsPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
