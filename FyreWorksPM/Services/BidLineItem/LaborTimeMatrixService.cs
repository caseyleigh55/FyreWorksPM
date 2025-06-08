using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.Services.BidLineItem
{
    public static class LaborTimeMatrixService
    {
        private static readonly Dictionary<string, Dictionary<string, decimal>> _hourMatrix =
            new()
            {
                ["warehouse"] = new() { ["normal"] = 0.5m, ["lift"] = 1.0m, ["panel"] = 1.0m, ["pipe"] = 0.5m },
                ["hardlid"] = new() { ["normal"] = 0.5m, ["lift"] = 1.0m, ["panel"] = 1.0m, ["pipe"] = 0.5m },
                ["tbar"] = new() { ["normal"] = 0.25m, ["lift"] = 0.5m, ["panel"] = 1.0m, ["pipe"] = 1.0m },
                ["underground"] = new() { ["normal"] = 0.25m, ["lift"] = 1.5m, ["panel"] = 1.0m, ["pipe"] = 1.5m },
                ["panel room"] = new() { ["normal"] = 0.25m, ["lift"] = 1.0m, ["panel"] = 1.0m, ["pipe"] = 1.0m },
                ["demo"] = new() { ["normal"] = 0.5m },
                ["trim"] = new() { ["normal"] = 0.5m, ["lift"] = 1.0m, ["panel"] = 1.0m, ["pipe"] = 1.0m }
            };

        public static decimal? GetHours(string location, string installType)
        {
            if (string.IsNullOrWhiteSpace(location) || string.IsNullOrWhiteSpace(installType))
                return null;

            var loc = location.Trim().ToLowerInvariant();
            var type = installType.Trim().ToLowerInvariant();

            if (_hourMatrix.TryGetValue(loc, out var typeDict) &&
                typeDict.TryGetValue(type, out var hours))
            {
                return hours;
            }

            return null;
        }
    }


}
