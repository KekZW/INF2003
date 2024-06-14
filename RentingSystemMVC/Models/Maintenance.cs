using System.ComponentModel.DataAnnotations;

namespace RentingSystemMVC.Models
{
    public class Maintenance
    {
        [Key]
        public int MaintenanceID { get; set; }
        public int VehicleID { get; set; }
        public string WorkshopStatus { get; set; }
        public DateOnly FinishMaintDate { get; set; }
        public DateOnly LastMaintDate { get; set; }
        
        // Navigation Property
        public Vehicle Vehicle { get; set; }
    }
}
