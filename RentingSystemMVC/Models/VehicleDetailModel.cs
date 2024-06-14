namespace RentingSystemMVC.Models;

public class VehicleDetailModel: VehicleViewModel
{
    public VehicleViewModel Vehicle { get; set; }
    public List<Maintenance> Maintenances { get; set; }
}