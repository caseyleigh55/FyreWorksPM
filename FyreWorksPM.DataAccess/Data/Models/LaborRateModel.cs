using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.DataAccess.Data.Models
{
    public class LaborRateModel
    {
        public Guid Id { get; set; }
        public string Role { get; set; } = ""; // Journeyman / Apprentice

        public decimal RegularDirectRate { get; set; }
        public decimal RegularBilledRate { get; set; }
        public decimal OvernightDirectRate { get; set; }
        public decimal OvernightBilledRate { get; set; }

        public Guid LaborTemplateId { get; set; }
        public LaborTemplateModel LaborTemplateModel { get; set; }
    }

}
