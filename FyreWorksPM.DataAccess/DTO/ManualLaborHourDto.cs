using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.DataAccess.DTO
{
    public class ManualLaborHourDto
    {
        public int Id { get; set; }              // Unique identifier for the record
        public string Role { get; set; }          // "Journeyman", "Apprentice"
        public string Category { get; set; }      // "Demo", "Trim", etc.
        public decimal Hours { get; set; }        // The actual number of hours
    }

}
