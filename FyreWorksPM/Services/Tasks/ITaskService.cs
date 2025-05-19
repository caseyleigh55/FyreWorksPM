using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Enums;

namespace FyreWorksPM.Services.Tasks
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetTemplatesByTypeAsync(TaskType type);
        Task<TaskDto> CreateTemplateAsync(CreateTaskDto dto);
        Task UpdateTaskAsync(int id, TaskDto dto);
        Task DeleteTaskAsync(int id);
    }
}
