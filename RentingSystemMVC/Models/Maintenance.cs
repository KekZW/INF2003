using System.ComponentModel.DataAnnotations;

namespace RentingSystemMVC.Models
{
    public class Maintenance
    {
        [Key]
        public int MaintenenceID { get; set; }
        public int VehicleID { get; set; }
        public string WorkshopStatus { get; set; }
        public DateOnly FinishMaintDate { get; set; }
        public DateOnly LastMaintDate { get; set; }
    }
}
