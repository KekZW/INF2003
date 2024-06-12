namespace RentingSystemMVC.Models
{
    public class Maintenance
    {
        public int VehicleID { get; set; }
        public string WorkshopStatus { get; set; }
	      public date FinishMaintDate { get; set; }
	      public date LastMaintDate { get; set; }
    }
}
