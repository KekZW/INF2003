using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using System.Windows.Input;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using RentingSystemMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace RentingSystemMVC.Controllers
{
    public class RentalController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Database=vehicleDB;Uid=root;Pwd=;";
        
        [Authorize(Roles="User")]
        public IActionResult Index()
        {
            List<Rental> rentals = new List<Rental>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT r.rentalID, r.userID, r.vehicleID, v.licensePlate, " +
                                "r.startRentalDate, r.endRentalDate, r.rentalAmount, r.rentalAddress, r.rentalLot " +
                                "FROM rental r INNER JOIN vehicle v ON r.vehicleID = v.vehicleID " +
                                "WHERE r.userID = @userID AND @todayDate < r.endRentalDate";


                int userID = GetCurrentUserID();

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userID", userID);
                    command.Parameters.AddWithValue("@todayDate", DateTime.Today);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            Rental rental = new Rental
                            {
                                rentalID = reader.GetInt32("rentalID"),
                                UserID = reader.GetInt32("UserID"),
                                vehicleID = reader.GetInt32("vehicleID"),
                                LicensePlate = reader.GetString("LicensePlate"),
                                startRentalDate = reader.GetDateTime("startRentalDate"),
                                endRentalDate = reader.GetDateTime("endRentalDate"),
                                rentalAmount = reader.GetDecimal("rentalAmount"),
                                rentalAddress = reader.GetString("rentalAddress"),
                                rentalLot = reader.GetInt32("rentalLot")

                            };

                            rentals.Add(rental);
                        }
                    }
                }
            }

            return View(rentals);
        }

        [HttpPost]
        public IActionResult DeleteRental(int rentalID)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM rental WHERE rentalID = @rentalID";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@rentalID", rentalID);
                        command.ExecuteNonQuery();
                    }

                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult EditRental(int rentalID, int vehicleID, DateTime startRentalDate, DateTime endRentalDate)
        {
            try
            {
                decimal cost = getRentalCost(vehicleID);
                Console.WriteLine($"Cost retrieved: {cost}");

                TimeSpan rentalDuration = endRentalDate - startRentalDate;
                int rentalDays = (int)rentalDuration.TotalDays;
                Console.WriteLine($"rental Days: {rentalDays}");

                cost = cost * rentalDays;
                Console.WriteLine($"Cost retrieved: {cost}");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query =
                        "UPDATE rental SET startRentalDate = @startRentalDate, " +
                        "endRentalDate = @endRentalDate, rentalAmount = @cost WHERE rentalID = @rentalID";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@startRentalDate", startRentalDate);
                        command.Parameters.AddWithValue("@endRentalDate", endRentalDate);
                        command.Parameters.AddWithValue("@rentalID", rentalID);
                        command.Parameters.AddWithValue("@cost", cost);
                        command.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true });
            }
            catch (MySqlException ex)
            {
                return Json(new { success = false });
            }
        }

        private int GetCurrentUserID()
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            string email = null;

            if (emailClaim == null)
                return -1;
            else
                email = emailClaim.Value;
            int userID = -1;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT userID FROM user WHERE emailAddress = @Email";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userID = reader.GetInt32("userID");
                        }
                    }
                }
            }
            return userID;
        }

        private decimal getRentalCost(int vehicleID)
        {
            decimal cost = 0;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT vt.rentalCostPerDay FROM vehicle v " +
                    "JOIN vehicletype vt ON v.vehicleTypeID = vt.vehicleTypeID " +
                    "WHERE v.vehicleID = @vehicleID";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@vehicleID", vehicleID);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cost = reader.GetDecimal("rentalCostPerDay");
                        }
                    }
                }
            }
            Console.WriteLine($"Cost retrieved: {cost}");
            return cost;
        }
    }
}
