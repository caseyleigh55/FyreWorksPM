namespace FyreWorksPM.Services.BidLineItem
{
    public static class TrimTimeLookupService
    {
        private static readonly Dictionary<string, decimal> _trimHoursByType =
            new()
            {
                ["normal"] = 0.5m,
                ["lift"] = 1.0m,
                ["panel"] = 1.0m,
                ["pipe"] = 1.0m
            };

        public static decimal GetHours(string installType)
        {
            if (string.IsNullOrWhiteSpace(installType))
                return 0;

            return _trimHoursByType.TryGetValue(installType.Trim().ToLowerInvariant(), out var hours)
                ? hours
                : 0;
        }
    }


}
