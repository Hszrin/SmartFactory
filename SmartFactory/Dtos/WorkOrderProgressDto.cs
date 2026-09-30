using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactory.Dtos
{
    public class WorkOrderProgressDto
    {
        public int WorkOrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public int TargetQuantity { get; set; }
        public int CurrentQuantity { get; set; }

        public double ProgressRate { get; set; }
    }
}
