using FyreWorksPM.DataAccess.DTO; // Adjust if DTOs live elsewhere

namespace FyreWorksPM.Services.Bid
{
    /// <summary>
    /// Defines operations for working with Bids via Web API.
    /// </summary>
    public interface IBidService
    {
        /// <summary>
        /// Gets the next available bid number (e.g., "B-001").
        /// </summary>
        Task<string> GetNextBidNumberAsync();

        /// <summary>
        /// Creates a new bid.
        /// </summary>
        Task<BidDto> CreateBidAsync(CreateBidDto dto);

        /// <summary>
        /// Retrieves all bids.
        /// </summary>
        Task<List<BidDto>> GetAllBidsAsync();

        /// <summary>
        /// Retrieves a single bid by its ID.
        /// </summary>
        Task<BidDto?> GetBidByIdAsync(int id);

        /// <summary>
        /// Updates an existing bid.
        /// </summary>
        Task<bool> UpdateBidAsync(int id, BidDto dto);
    }
}
