using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.DataAccess.Enums;
using System.Diagnostics;
using System.Net.Http.Json;

namespace FyreWorksPM.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly HttpClient _http;

        public TaskService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TaskDto>> GetTemplatesByTypeAsync(TaskType type)
        {
            var tasks = await _http.GetFromJsonAsync<List<TaskDto>>(
                $"api/tasks/templates?type={(int)type}")
                ?? new List<TaskDto>();

            Debug.WriteLine($"Found {tasks.Count} tasks from API.");

            return tasks;
        }

        public async Task<TaskDto> CreateTemplateAsync(CreateTaskDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/tasks/templates", dto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TaskDto>()
                   ?? throw new InvalidOperationException("Failed to parse task template response.");
        }

        public async Task UpdateTaskAsync(int id, TaskDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/tasks/templates/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/tasks/templates/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
