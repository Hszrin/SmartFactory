using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartFactory.Models
{
    [Table("ProductionLine")]
    public class ProductionLine
    {
        [Key]
        public int LineId { get; set; }

        public string LineCode { get; set; } = string.Empty;

        public string LineName { get; set; } = string.Empty;

        public string Status { get; set; } = "STOP";

        public DateTime CreatedAt { get; set; }
    }
}