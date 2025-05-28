namespace FyreWorksPM.DataAccess.Data.Models
{
    public class BidLaborConfig
    {
        public Guid BidId { get; set; }

        // Prewire hours per location + install type
        public Dictionary<string, Dictionary<string, double>> PrewireHours { get; set; } = new();

        // Trim hours per install type
        public Dictionary<string, double> TrimHours { get; set; } = new();

        // Optional: Demo hours per install type
        public Dictionary<string, double> DemoHours { get; set; } = new();
    }
}
