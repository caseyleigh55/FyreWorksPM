using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.DataAccess.Data.Models
{
    public class LocationHourModel
    {
        public Guid Id { get; set; }
        public string LocationName { get; set; } = "";

        public decimal Normal { get; set; }
        public decimal Lift { get; set; }
        public decimal Panel { get; set; }
        public decimal Pipe { get; set; }

        public Guid LaborTemplateId { get; set; }
        public LaborTemplateModel LaborTemplateModel { get; set; }
    }

}
