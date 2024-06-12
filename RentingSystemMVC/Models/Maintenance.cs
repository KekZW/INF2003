namespace RentingSystemMVC.Models
{
    public class Maintenance
    {
        public int VehicleID { get; set; }
        public string WorkshopStatus { get; set; }
        public DateOnly FinishMaintDate { get; set; }
        public DateOnly LastMaintDate { get; set; }
    }
}
