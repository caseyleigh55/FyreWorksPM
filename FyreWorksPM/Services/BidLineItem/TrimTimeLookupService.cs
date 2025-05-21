namespace FyreWorksPM.Services.BidLineItem
{
    public static class TrimTimeLookupService
    {
        private static readonly Dictionary<string, int> _trimTimes = new()
        {
            ["Normal"] = 15,
            ["Lift"] = 30,
            ["Panel"] = 45,
            ["Pipe"] = 60
        };

        public static int GetMinutes(string installType)
        {
            if (string.IsNullOrWhiteSpace(installType))
                return 0;

            return _trimTimes.TryGetValue(installType.Trim(), out var minutes)
                ? minutes
                : 0;
        }
    }

}
