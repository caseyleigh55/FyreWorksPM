using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.DataAccess.DTO
{
    public class ManualLaborHourDto
    {
        public string Role { get; set; }          // "Journeyman", "Apprentice"
        public string Location { get; set; }      // "Demo", "Trim", etc.
        public decimal Hours { get; set; }        // The actual number of hours
        public bool IsOvernight { get; set; }     // Optional, if you separate night/day
    }

}
