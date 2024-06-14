namespace RentingSystemMVC.Models
{
    public class Vehicle
    {
        public int VehicleID { get; set; }
        public string LicensePlate { get; set; }
        public string LicenseToOperate { get; set; }
        public int VehicleTypeID { get; set; }
        
        // Navigation Property
        public VehicleType VehicleType { get; set; }
    }
}




