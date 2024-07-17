using System.ComponentModel.DataAnnotations;

namespace RentingSystemMVC.Models
{
    public class Maintenance
    {
        [Key]
        public int MaintenanceID { get; set; }
        public int VehicleID { get; set; }
        public string? WorkshopStatus { get; set; }
        public DateOnly startMaintDate { get; set; }
        public DateOnly endMaintDate { get; set; }
        
        // Navigation Property
        public Vehicle? Vehicle { get; set; }
    }
}