using System.Collections.Generic;
using System.Security.Claims;
using System.Windows.Input;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using RentingSystemMVC.Models;

namespace RentingSystem.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Database=vehicleDB;Uid=root;Pwd=;";

        // GET: Vehicles/Index
        public IActionResult Index(DateTime? selectedDate, string filterColumn, string filterValue)
        {
            List<Vehicle> vehicles = new List<Vehicle>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT v.vehicleID, v.licensePlate, v.licenseToOperate, vt.brand, vt.model, vt.type, " +
                       "vt.seats, vt.fuelCapacity, vt.fuelType, vt.truckSpace, vt.rentalCostPerDay " +
                       "FROM vehicle v " +
                       "INNER JOIN vehicleType vt ON v.vehicleTypeID = vt.vehicleTypeID " +
                       "LEFT JOIN rental r ON v.vehicleID = r.vehicleID " +
                       "AND r.startRentalDate <= @todayDate " +
                       "AND r.endRentalDate >= @todayDate " +
                       "LEFT JOIN maintenanace m ON v.vehicleID = m.vehicleID " +
                       "AND m.finishMaintDate <= @todayDate " +
                       "AND m.workshopStatus != 'Completed' " +
                       "WHERE r.vehicleID IS NULL AND m.vehicleID IS NULL";

                //Need change maintenanace workshopStatus value

                if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
                {
                    query += " AND " + filterColumn + " LIKE @filterValue";
                }

                using (var command = new MySqlCommand(query, connection))
                {

                    if (selectedDate.HasValue)
                    {
                        command.Parameters.AddWithValue("@todayDate", selectedDate);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@todayDate", DateTime.Today);
                    }

                    if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
                    {
                        command.Parameters.AddWithValue("@filterValue", "%" + filterValue + "%");
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            Vehicle vehicle = new Vehicle
                            {
                                VehicleID = reader.GetInt32("vehicleID"),
                                LicensePlate = reader.GetString("licensePlate"),
                                LicenseToOperate = reader.GetString("licenseToOperate"),
                                Brand = reader.GetString("brand"),
                                Model = reader.GetString("model"),
                                Type = reader.GetString("type"),
                                Seats = reader.GetInt32("seats"),
                                FuelCapacity = reader.GetDecimal("fuelCapacity"),
                                FuelType = reader.GetString("fuelType"),
                                TruckSpace = reader.GetDecimal("truckSpace"),
                                RentalCostPerDay = reader.GetDecimal("rentalCostPerDay")
                            };

                            vehicles.Add(vehicle);
                        }
                    }
                }
            }

            return View(vehicles);
        }
        [HttpPost]
        public JsonResult CheckAvailability(int vehicleID, DateTime startRentalDate, DateTime endRentalDate)
        {
            bool available = true;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM rental " +
                               "WHERE vehicleID = @vehicleID " +
                               "AND (startRentalDate <= @endRentalDate AND endRentalDate >= @startRentalDate)";

                //Need change maintenanace workshopStatus value

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@vehicleID", vehicleID);
                    command.Parameters.AddWithValue("@startRentalDate", startRentalDate);
                    command.Parameters.AddWithValue("@endRentalDate", endRentalDate);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    if (count > 0)
                    {
                        available = false;
                    }
                }

                if (available)
                {
                    query = "SELECT COUNT(*) FROM maintenanace " +
                            "WHERE vehicleID = @vehicleID " +
                            "AND (finishMaintDate > @endRentalDate AND finishMaintDate > @startRentalDate) " +
                            "AND workshopStatus != 'Completed'";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@vehicleID", vehicleID);
                        command.Parameters.AddWithValue("@startRentalDate", startRentalDate);
                        command.Parameters.AddWithValue("@endRentalDate", endRentalDate);

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        if (count > 0)
                        {
                            available = false;
                        }
                    }
                }
            }

            return Json(new { available });
        }

        [HttpPost]
        public IActionResult RentVehicle(int vehicleID, DateTime startRentalDate, DateTime endRentalDate, string rentalAddress, int rentalLot, decimal rentalAmount)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO rental (userID, vehicleID, startRentalDate, endRentalDate, rentalAmount, rentalAddress, rentalLot) " +
                               "VALUES (@userID, @vehicleID, @startRentalDate, @endRentalDate, @rentalAmount, @rentalAddress, @rentalLot)";

                using (var command = new MySqlCommand(query, connection))
                {
                    int userID = GetCurrentUserID();

                    command.Parameters.AddWithValue("@userID", userID);
                    command.Parameters.AddWithValue("@vehicleID", vehicleID);
                    command.Parameters.AddWithValue("@startRentalDate", startRentalDate);
                    command.Parameters.AddWithValue("@endRentalDate", endRentalDate);
                    command.Parameters.AddWithValue("@rentalAmount", rentalAmount);
                    command.Parameters.AddWithValue("@rentalAddress", rentalAddress);
                    command.Parameters.AddWithValue("@rentalLot", rentalLot);

                    command.ExecuteNonQuery();
                }
            }

            return Json(new { success = true });
        }

        private int GetCurrentUserID()
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim == null)
            {
                throw new Exception("User email claim not found.");
            }

            string email = emailClaim.Value;
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

            if (userID == -1)
            {
                throw new Exception("User ID not found.");
            }

            return userID;
        }

    }
}


