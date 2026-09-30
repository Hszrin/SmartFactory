using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactory.Dtos
{
    public class DashboardSummaryDto
    {
        public int TotalProduction { get; set; }
        public int GoodQuantity { get; set; }
        public int DefectQuantity { get; set; }
        public double DefectRate { get; set; }

        public int RunningWorkOrderCount { get; set; }
        public int CompletedWorkOrderCount { get; set; }
    }
}
