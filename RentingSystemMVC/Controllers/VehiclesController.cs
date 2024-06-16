using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using System.Windows.Input;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using RentingSystemMVC.Data;
using RentingSystemMVC.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RentingSystem.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Database=vehicleDB;Uid=root;Pwd=;";

        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Vehicles/Index
        public IActionResult Index(DateTime? selectedDate, string? filterColumn, string? filterValue)
        {
            List<AuthorisedVehicleView> vehicles = new List<AuthorisedVehicleView>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query =
                    "SELECT v.vehicleID, v.licensePlate, v.licenseToOperate, vt.brand, vt.model, vt.type, " +
                    "vt.seats, vt.fuelCapacity, vt.fuelType, vt.truckSpace, vt.rentalCostPerDay, COUNT(r.vehicleID) AS timesRented " +
                    "FROM vehicle v " +
                    "INNER JOIN vehicleType vt ON v.vehicleTypeID = vt.vehicleTypeID " +
                    "LEFT JOIN rental r ON v.vehicleID = r.vehicleID " + 
                    "WHERE v.vehicleID NOT IN" +
                    "(SELECT * FROM RentalInProgress)"+
                    "AND v.vehicleID NOT IN" +
                    "(SELECT * FROM MaintenanceInProgress)";
                    

                if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
                {
                    query += " AND vt." + filterColumn + " LIKE @filterValue";
                }

                query += " GROUP BY v.vehicleID ORDER BY COUNT(r.vehicleID) DESC";

                using (var command = new MySqlCommand(query, connection))
                {
                    if (selectedDate.HasValue)
                    {
                        command.Parameters.AddWithValue("@todayDate", selectedDate);
                    }

                    if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
                    {
                        command.Parameters.AddWithValue("@filterValue", "%" + filterValue + "%");
                    }

                   
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AuthorisedVehicleView vehicle = new AuthorisedVehicleView()
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
                                RentalCostPerDay = reader.GetDecimal("rentalCostPerDay"),
                            };

                            vehicle.IsUserAuthorized = ableToOperate(vehicle.LicenseToOperate);

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
                    query = "SELECT COUNT(*) FROM maintenance " +
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
        public IActionResult RentVehicle(int vehicleID, string licenseToOperate, DateTime startRentalDate,
            DateTime endRentalDate, string rentalAddress,
            int rentalLot, decimal rentalAmount)
        {

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query =
                    "INSERT INTO rental (userID, vehicleID, startRentalDate, endRentalDate, rentalAmount, rentalAddress, rentalLot) " +
                    "VALUES (@userID, @vehicleID, @startRentalDate, @endRentalDate, @rentalAmount, @rentalAddress, @rentalLot)";

                try
                {
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

                    return Json(new { success = true });
                }

                catch (MySqlException ex)
                {

                    return Json(new { success = false });
                }
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

        public bool ableToOperate(string licenseToOperate)
        {
            String userlicenseToOperate = GetCurrentUserlicenseToOperate();
            if (licenseToOperate == "3A" && userlicenseToOperate == "3")
            {
                return true;
            }

            return userlicenseToOperate == licenseToOperate;
        }

        private string GetCurrentUserlicenseToOperate()
        {
            int userID = GetCurrentUserID();
            String licenseToOperate = null;

            if (userID > 0)
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT licenseClass FROM license WHERE userID = @userID";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userID", userID);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                licenseToOperate = reader.GetString("licenseClass");
                            }
                        }
                    }

                    return licenseToOperate;
                }
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        public IActionResult Manage(string searchTerm)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            
            string query = "SELECT v.vehicleID, v.licensePlate, v.licenseToOperate, vt.brand, vt.model, vt.type, " +
                           "vt.seats, vt.fuelCapacity, vt.fuelType, vt.truckSpace, vt.rentalCostPerDay " +
                           "FROM vehicle v " +
                           "INNER JOIN vehicleType vt ON v.vehicleTypeID = vt.vehicleTypeID ";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query +=
                    " WHERE (v.licensePlate LIKE {0} OR vt.brand LIKE {0} OR vt.model LIKE {0} OR CONCAT(vt.brand, ' ', vt.model) LIKE {0})";
            }

            query += "GROUP BY v.vehicleID";


            List <VehicleViewModel> vehicleList = _context.VehicleViewModel.FromSqlRaw(query, "%" + searchTerm + "%").ToList();
            return View(vehicleList);
        }
        
        public IActionResult Details(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            
            // TODO: Retrieve maintenance logs for the vehicle, combine with vehicleViewModel 
            string maintenanceQuery = "SELECT * FROM maintenance WHERE vehicleID = @p0";

            List<Maintenance> maintenanceLogs = null;

            if (_context.Maintenance != null)
            {
                maintenanceLogs = _context.Maintenance.FromSqlRaw(maintenanceQuery, id).ToList();
            }

            string vehQuery = "SELECT v.vehicleID, v.licensePlate, v.licenseToOperate, vt.brand, vt.model, vt.type, " +
                              "vt.seats, vt.fuelCapacity, vt.fuelType, vt.truckSpace, vt.rentalCostPerDay " +
                              "FROM vehicle v " +
                              "INNER JOIN vehicleType vt ON v.vehicleTypeID = vt.vehicleTypeID " +
                              "WHERE v.vehicleID = @p0";

            VehicleViewModel vehicle = _context.VehicleViewModel
                .FromSqlRaw(vehQuery, id)
                .FirstOrDefault();

            Console.WriteLine(maintenanceLogs);

            VehicleDetailModel vehicleDetail = new VehicleDetailModel
                { Maintenances = maintenanceLogs, Vehicle = vehicle };
            return View(vehicleDetail);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateVehicle()
        {
            CreateVehicleViewModel model = new CreateVehicleViewModel();
            model.VehicleTypes = _context.VehicleType.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateVehicle(CreateVehicleViewModel model)
        {   

            if (ModelState.IsValid)
            {   
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO vehicle (LicensePlate, LicenseToOperate, VehicleTypeID) " +
                                   "VALUES (@LicensePlate, @LicenseToOperate, @VehicleTypeID)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LicensePlate",  model.LicensePlate);
                        command.Parameters.AddWithValue("@LicenseToOperate", model.LicenseToOperate);
                        command.Parameters.AddWithValue("@VehicleTypeID", model.VehicleTypeID);

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }
            return RedirectToAction("CreateVehicle");
        }

        [HttpPost]
        public IActionResult AddMaintenance(int vehicleID, DateTime startDate, DateTime endDate, string description)
        {
            using(var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO maintenance (vehicleID, workshopStatus, finishMaintDate, LastMaintDate) "
                + "VALUES (@vehicleID, @description, @startDate, @endDate)";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@vehicleID", vehicleID);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);

                    command.ExecuteNonQuery();
                }
            }
            
            return Json(new { success = true });
        }
    }
}
