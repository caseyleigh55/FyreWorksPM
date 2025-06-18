using FyreWorksPM.DataAccess.DTO;
using System.Net.Http.Json;

namespace FyreWorksPM.Services.Labor
{
    public class BidLaborTemplateService : IBidLaborTemplateService
    {
        private readonly HttpClient _httpClient;

        public BidLaborTemplateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SaveBidLaborTemplateAsync(BidLaborTemplateDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/bidlabortemplate", dto);
            response.EnsureSuccessStatusCode(); // throw if not 200-299
        }
    }
}
