namespace RentingSystemMVC.Models;

public class SimpleVehicleViewModel
{
    public int VehicleID { get; set; }
    public string LicensePlate { get; set; }
    public string LicenseToOperate { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string Type { get; set; }
    public int Seats { get; set; }
    public decimal FuelCapacity { get; set; }
    public string FuelType { get; set; }
    public decimal TruckSpace { get; set; }
    public decimal RentalCostPerDay { get; set; }
    
    public int TimesRented { get; set; }

}