using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.DataAccess.DTO
{
    public class LocationHourDto
    {
        public string LocationName { get; set; }

        public decimal Normal { get; set; }
        public decimal Lift { get; set; }
        public decimal Panel { get; set; }
        public decimal Pipe { get; set; }
    }

}
