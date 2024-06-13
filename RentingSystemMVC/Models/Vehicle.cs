using System.ComponentModel.DataAnnotations;

namespace RentingSystemMVC.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleID { get; set; }
        public string LicensePlate { get; set; }
        public string LicenseToOperate { get; set; }
        public string VehicleTypeId { get; set; }
        
        // Navigation Property
        public VehicleType VehicleType { get; set; }
        
    }
}




