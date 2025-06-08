using FyreWorksPM.Configuration;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;
using System.Net.Http.Json;
using System.Text.Json;

namespace FyreWorksPM.Services.Labor
{
    public class LaborTemplateService : ILaborTemplateService
    {
        private readonly HttpClient _httpClient;

        public LaborTemplateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LaborTemplateDto>> GetAllTemplatesAsync()
        {
            var response = await _httpClient.GetAsync("api/LaborTemplates");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<LaborTemplateDto>>();
        }

        public async Task<LaborTemplateDto?> GetTemplateByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/LaborTemplates/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<LaborTemplateDto>();
        }

        public async Task<LaborTemplateDto?> CreateTemplateAsync(LaborTemplateDto template)
        {
            var response = await _httpClient.PostAsJsonAsync("api/LaborTemplates", template);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                {
                    await Application.Current.MainPage.DisplayAlert("Client Error", content, "OK");
                    return null;
                }

                throw new Exception($"❌ API Error: {response.StatusCode}\nBody:\n{content}");
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                return JsonSerializer.Deserialize<LaborTemplateDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                throw new Exception($"⚠️ JSON parse error.\nResponse Body:\n{json}", ex);
            }
        }

        public async Task DeleteTemplateAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/LaborTemplates/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
