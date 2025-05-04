using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.Services.Bid
{
    public class BidService : IBidService
    {
        private readonly HttpClient _httpClient;

        public BidService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetNextBidNumberAsync()
        {
            return await _httpClient.GetStringAsync("api/bids/next-number");
        }

        public async Task<BidDto> CreateBidAsync(CreateBidDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/bids", dto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BidDto>();
        }
    }
}
