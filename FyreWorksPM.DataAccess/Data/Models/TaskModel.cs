using FyreWorksPM.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.DataAccess.Data.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public TaskType Type { get; set; }

        public ICollection<BidTaskModel> BidTasks { get; set; }
    }

}
