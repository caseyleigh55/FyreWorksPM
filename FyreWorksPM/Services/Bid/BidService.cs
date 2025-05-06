using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FyreWorksPM.DataAccess.DTO; // Adjust if DTOs are elsewhere

namespace FyreWorksPM.Services.Bid
{
    /// <summary>
    /// Service responsible for interacting with the Bids Web API endpoints.
    /// </summary>
    public class BidService : IBidService
    {
        private readonly HttpClient _httpClient;

        public BidService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves the next available bid number (e.g., "B-001").
        /// </summary>
        public async Task<string> GetNextBidNumberAsync()
        {
            return await _httpClient.GetStringAsync("api/bids/next-number");
        }

        /// <summary>
        /// Creates a new bid using POST /api/bids.
        /// </summary>
        public async Task<BidDto> CreateBidAsync(CreateBidDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/bids", dto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BidDto>();
        }

        /// <summary>
        /// Gets all bids from the API.
        /// </summary>
        public async Task<List<BidDto>> GetAllBidsAsync()
        {
            try
            {
                var bids = await _httpClient.GetFromJsonAsync<List<BidDto>>("api/bids");
                return bids ?? new List<BidDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all bids: {ex.Message}");
                return new List<BidDto>();
            }
        }

        /// <summary>
        /// Retrieves a single bid by ID.
        /// </summary>
        public async Task<BidDto?> GetBidByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<BidDto>($"api/bids/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching bid {id}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Updates an existing bid using PUT /api/bids/{id}.
        /// </summary>
        public async Task<bool> UpdateBidAsync(int id, BidDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/bids/{id}", dto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating bid {id}: {ex.Message}");
                return false;
            }
        }
    }
}
