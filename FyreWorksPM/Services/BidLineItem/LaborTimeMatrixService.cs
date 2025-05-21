using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.Services.BidLineItem
{
    public static class LaborTimeMatrixService
    {
        private static readonly Dictionary<string, Dictionary<string, int>> _matrix = new()
        {
            ["warehouse"] = new() { ["Normal"] = 30, ["Lift"] = 60, ["Pipe"] = 60 },
            ["hardlid"] = new() { ["Normal"] = 30, ["Lift"] = 60, ["Pipe"] = 60 },
            ["tbar"] = new() { ["Normal"] = 15, ["Lift"] = 60, ["Pipe"] = 60 },
            ["underground"] = new() { ["Pipe"] = 60 },
            ["panel room"] = new() { ["Panel"] = 60 }
        };

        public static int? GetMinutes(string location, string installType)
        {
            if (string.IsNullOrWhiteSpace(location) || string.IsNullOrWhiteSpace(installType))
                return null;

            location = location.Trim().ToLowerInvariant();
            installType = installType.Trim();

            return _matrix.TryGetValue(location, out var installMap) &&
                   installMap.TryGetValue(installType, out var minutes)
                   ? minutes
                   : null;
        }
    }

}
