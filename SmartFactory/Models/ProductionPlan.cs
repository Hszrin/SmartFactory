using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactory.Models
{
    [Table("ProductionPlan")]
    public class ProductionPlan
    {
        [Key]
        public int PlanId { get; set; }
        public int ProductId { get; set; }
        public int LineId { get; set; }
        public int TargetQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Product? Product { get; set; }
        public ProductionLine? Line { get; set; }
    }
}
