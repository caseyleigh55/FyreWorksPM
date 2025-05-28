namespace FyreWorksPM.Services.BidLineItem
{
    public static class TrimTimeLookupService
    {
        private static readonly Dictionary<string, double> _trimHoursByType =
            new()
            {
                ["normal"] = 0.5,
                ["lift"] = 1.0,
                ["panel"] = 1.0,
                ["pipe"] = 1.0
            };

        public static double GetHours(string installType)
        {
            if (string.IsNullOrWhiteSpace(installType))
                return 0;

            return _trimHoursByType.TryGetValue(installType.Trim().ToLowerInvariant(), out var hours)
                ? hours
                : 0;
        }
    }


}
