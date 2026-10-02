using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartFactory.Models
{
    [Table("Defect")]
    public class Defect
    {
        [Key]
        public int DefectId { get; set; }
        public long ResultId { get; set; }
        public string DefectType { get; set; } = string.Empty;
        public int DefectQuantity { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ProductionResult? Result { get; set; }
    }
}
