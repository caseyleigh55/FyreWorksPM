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

        public async Task<List<SavedTaskDto>> GetTemplatesByTypeAsync(TaskType type)
        {
            var tasks = await _http.GetFromJsonAsync<List<SavedTaskDto>>(
                $"api/tasks/templates?type={(int)type}")
                ?? new List<SavedTaskDto>();

            Debug.WriteLine($"Found {tasks.Count} tasks from API.");

            return tasks;
        }

        public async Task<SavedTaskDto> CreateTemplateAsync(CreateTaskDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/tasks/templates", dto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<SavedTaskDto>()
                   ?? throw new InvalidOperationException("Failed to parse task template response.");
        }

        public async Task UpdateTemplateAsync(int id, CreateTaskDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/tasks/templates/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTemplateAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/tasks/templates/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
