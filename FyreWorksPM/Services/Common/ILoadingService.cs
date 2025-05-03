using System.Threading.Tasks;

namespace FyreWorksPM.Services.Common
{
    public interface ILoadingService
    {
        Task ShowAsync();
        Task HideAsync();
    }
}
