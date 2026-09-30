using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartFactory.Models
{
    [Table("WorkOrder")]
    public class WorkOrder
    {
        [Key]
        public int WorkOrderId { get; set; }

        public int PlanId { get; set; }

        public int ProductId { get; set; }

        public int LineId { get; set; }

        public int TargetQuantity { get; set; }

        public string Status { get; set; } = "WAITING";

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public ProductionPlan? Plan { get; set; }
        public Product? Product { get; set; }
        public ProductionLine? Line { get; set; }

        [NotMapped]
        public int CurrentQuantity { get; set; }
    }
}