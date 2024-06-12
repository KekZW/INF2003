namespace RentingSystemMVC.Models
{
    public class Rental
    {
        public int rentalID { get; set; }
        public int UserID { get; set; }
        public int vehicleID { get; set; }
        public string LicensePlate { get; set; }
        public DateTime startRentalDate { get; set; }
        public DateTime endRentalDate { get; set; }
        public decimal rentalAmount { get; set; }
        public string rentalAddress { get; set; }
        public int rentalLot { get; set; }
    }
}
