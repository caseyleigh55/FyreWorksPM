﻿using FyreWorksPM.DataAccess.Enums;

namespace FyreWorksPM.DataAccess.DTO
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public TaskType Type { get; set; }
        public decimal DefaultCost { get; set; }
        public decimal DefaultSale { get; set; }

        public override string ToString()
        {
            return TaskName;
        }

    }


}
