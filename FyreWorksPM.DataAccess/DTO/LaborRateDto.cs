using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.DataAccess.DTO
{
    public class LaborRateDto
    {
        public string Role { get; set; } // "Journeyman" or "Apprentice"

        public decimal RegularDirectRate { get; set; }
        public decimal RegularBilledRate { get; set; }
        public decimal OvernightDirectRate { get; set; }
        public decimal OvernightBilledRate { get; set; }
    }

}
