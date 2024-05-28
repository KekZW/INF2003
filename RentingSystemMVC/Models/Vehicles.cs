namespace RentingSystemMVC.Models
{
    public class Vehicles
    {
        public int VehicleID { get; set; }
        public string VehicleName { get; set; }
        public string Description { get; set; }
        public string LicensePlate { get; set; }
        public string FuelType { get; set; }
        public float FuelTankCapacity { get; set; }
        public float FuelLevel { get; set; }
        public string LicenseToOperate { get; set; }
    }
}

