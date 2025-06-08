namespace FyreWorksPM.DataAccess.Data.Models
{
    public class BidLaborConfig
    {
        public Guid BidId { get; set; }

        // Prewire hours per location + install type
        public Dictionary<string, Dictionary<string, decimal>> PrewireHours { get; set; } = new();

        // Trim hours per install type
        public Dictionary<string, decimal> TrimHours { get; set; } = new();

        // Optional: Demo hours per install type
        public Dictionary<string, decimal> DemoHours { get; set; } = new();
    }
}
