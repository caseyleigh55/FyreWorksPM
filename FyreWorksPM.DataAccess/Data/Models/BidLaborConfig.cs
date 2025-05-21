namespace FyreWorksPM.DataAccess.Data.Models
{
    public class BidLaborConfig
    {
        public Guid BidId { get; set; }

        // Per-location & install-type override
        public Dictionary<string, Dictionary<string, int>> PrewireMinutes { get; set; } = new();

        // Per-install-type override
        public Dictionary<string, int> TrimMinutes { get; set; } = new();
    }

}
