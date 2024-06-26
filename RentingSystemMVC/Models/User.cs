namespace RentingSystemMVC.Models;

public class User
{
    public int UserID { get; set; }
    public string UserPassword { get; set; }
    public string name { get; set; }
    public string Address { get; set; }
    public int? LicenseID { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNo { get; set; }
    public string Role { get; set; } = "User";
}