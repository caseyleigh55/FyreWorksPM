using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.DataAccess.Data.Models
{
    public class LaborTemplateModel
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;


        public ICollection<LaborRateModel> LaborRates { get; set; } = new List<LaborRateModel>();
        public ICollection<LocationHourModel> LocationHours { get; set; } = new List<LocationHourModel>();
    }

}
