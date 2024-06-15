namespace RentingSystemMVC.Models;

public class License
{
    public int LicenseID { get; set; }
    public DateTime AcquireDate { get; set; }
    public string LicenseClass { get; set; }
    public int? UserID { get; set; } 
}