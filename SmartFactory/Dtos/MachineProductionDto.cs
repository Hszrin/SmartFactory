using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactory.Dtos
{
    public class MachineProductionDto
    {
        public string MachineName { get; set; } = string.Empty;
        public int ProductionQuantity { get; set; }
    }
}
