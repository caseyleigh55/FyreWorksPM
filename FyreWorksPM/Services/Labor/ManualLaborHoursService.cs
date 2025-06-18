using FyreWorksPM.DataAccess.DTO;
using System.Net.Http.Json;

namespace FyreWorksPM.Services.Labor
{
    public class ManualLaborHourService : IManualLaborHourService
    {
        private readonly HttpClient _httpClient;

        public ManualLaborHourService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ManualLaborHourDto>> GetHoursByBidIdAsync(int bidId)
        {
            var response = await _httpClient.GetAsync($"api/manualLaborHours/{bidId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ManualLaborHourDto>>() ?? new();
        }

        public async Task SaveHoursAsync(Guid bidId, List<ManualLaborHourDto> hours)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/manualLaborHours/{bidId}", hours);
            response.EnsureSuccessStatusCode();
        }
    }

}
