using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Enums;

namespace FyreWorksPM.Services.Tasks
{
    public interface ITaskService
    {
        Task<List<SavedTaskDto>> GetTemplatesByTypeAsync(TaskType type);
        Task<SavedTaskDto> CreateTemplateAsync(CreateTaskDto dto);
        Task UpdateTemplateAsync(int id, CreateTaskDto dto);
        Task DeleteTemplateAsync(int id);
    }
}
