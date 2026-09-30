using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartFactory.Models
{
    [Table("Machine")]
    public class Machine
    {
        [Key]
        public int MachineId { get; set; }

        public string MachineCode { get; set; } = string.Empty;

        public string MachineName { get; set; } = string.Empty;

        public int LineId { get; set; }

        public string Status { get; set; } = "STOP";

        public DateTime CreatedAt { get; set; }

        public ProductionLine? Line { get; set; }
    }
}