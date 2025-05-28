using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.Services.BidLineItem
{
    public static class LaborTimeMatrixService
    {
        private static readonly Dictionary<string, Dictionary<string, double>> _hourMatrix =
            new()
            {
                ["warehouse"] = new() { ["normal"] = 0.5, ["lift"] = 1.0, ["panel"] = 1.0, ["pipe"] = 0.5 },
                ["hardlid"] = new() { ["normal"] = 0.5, ["lift"] = 1.0, ["panel"] = 1.0, ["pipe"] = 0.5 },
                ["tbar"] = new() { ["normal"] = 0.25, ["lift"] = 0.5, ["panel"] = 1.0, ["pipe"] = 1.0 },
                ["underground"] = new() { ["normal"] = 0.25, ["lift"] = 1.5, ["panel"] = 1.0, ["pipe"] = 1.5 },
                ["panel room"] = new() { ["normal"] = 0.25, ["lift"] = 1.0, ["panel"] = 1.0, ["pipe"] = 1.0 },
                ["demo"] = new() { ["normal"] = 0.5 },
                ["trim"] = new() { ["normal"] = 0.5, ["lift"] = 1.0, ["panel"] = 1.0, ["pipe"] = 1.0 }
            };

        public static double? GetHours(string location, string installType)
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
