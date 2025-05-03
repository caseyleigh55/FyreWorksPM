using CommunityToolkit.Maui.Views;
using FyreWorksPM.Pages.PopUps;
using FyreWorksPM.Extensions;

namespace FyreWorksPM.Services.Common
{
    public class LoadingService : ILoadingService
    {
        private LoadingPopup? _popup;

        public Task ShowAsync()
        {
            _popup = new LoadingPopup();

            var currentPage = Application.Current?.MainPage?.GetVisiblePage();

            if (currentPage is not null)
            {
                currentPage.ShowPopup(_popup);
            }
            else
            {
                Console.WriteLine("[LoadingService] No visible page found to show popup.");
            }

            return Task.CompletedTask; // ✅ to satisfy method signature
        }


        public async Task HideAsync()
        {
            if (_popup != null)
            {
                _popup.Close();
                _popup = null;
            }

            await Task.CompletedTask;
        }
    }
}
