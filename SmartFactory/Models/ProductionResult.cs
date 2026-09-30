using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartFactory.Models
{
    [Table("ProductionResult")]
    public class ProductionResult
    {
        [Key]
        public long ResultId { get; set; }

        public int WorkOrderId { get; set; }

        public int MachineId { get; set; }

        public int ProductionQuantity { get; set; }

        public int GoodQuantity { get; set; }

        public int DefectQuantity { get; set; }

        public DateTime ProductionTime { get; set; }

        public WorkOrder? WorkOrder { get; set; }

        public Machine? Machine { get; set; }
    }
}