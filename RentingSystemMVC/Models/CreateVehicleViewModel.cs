using Microsoft.AspNetCore.Mvc.Rendering;

namespace RentingSystemMVC.Models;
public class CreateVehicleViewModel
{
    public string LicensePlate { get; set; }
    public string LicenseToOperate { get; set; }
    public int VehicleTypeID { get; set; }
    public int? VehicleID { get; set; }
    public List<VehicleType>? VehicleTypes { get; set; }
}