using Microsoft.Maui.Controls.Platform.Compatibility; // might help depending on version
using Microsoft.Maui.Controls;

namespace FyreWorksPM.Extensions;

public static class PageExtensions
{
    public static Page? GetVisiblePage(this Page rootPage)
    {
        return rootPage switch
        {
            FlyoutPage flyout => flyout.Detail.GetVisiblePage(),
            NavigationPage nav => nav.CurrentPage?.GetVisiblePage(),
            TabbedPage tab => tab.CurrentPage?.GetVisiblePage(),
            Shell shell => (shell.CurrentItem?.CurrentItem as IShellSectionController)?.PresentedPage?.GetVisiblePage(),
            _ => rootPage
        };
    }
}
