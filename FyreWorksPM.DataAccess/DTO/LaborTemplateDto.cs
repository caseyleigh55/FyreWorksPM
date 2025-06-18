using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.DataAccess.DTO
{
   public class LaborTemplateDto
{
        public int Id { get; set; }

        public string TemplateName { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;

        public List<LocationHourDto> LocationHours { get; set; } = new();

        public List<LaborRateDto> LaborRates { get; set; } = new();
}

}
