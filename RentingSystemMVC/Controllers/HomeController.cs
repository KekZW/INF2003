using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using System.Windows.Input;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using RentingSystemMVC.Data;
using RentingSystemMVC.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RentingSystemMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString = "Server=localhost;Database=vehicleDB;Uid=root;Pwd=;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            
            List<SimpleVehicleViewModel> vehicles = new List<SimpleVehicleViewModel>();
            
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = 
                    "SELECT v.vehicleID, v.licensePlate, v.licenseToOperate, vt.brand, vt.model, vt.type, " +
                    "vt.seats, vt.fuelCapacity, vt.fuelType, vt.trunkSpace, vt.rentalCostPerDay, COUNT(r.vehicleID) AS timesRented " +
                    "FROM vehicle v " +
                    "INNER JOIN vehicleType vt ON v.vehicleTypeID = vt.vehicleTypeID " +
                    "LEFT JOIN rental r ON v.vehicleID = r.vehicleID " +
                    "GROUP BY v.vehicleID " +
                    "ORDER BY timesRented DESC " +
                    "LIMIT 8";

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SimpleVehicleViewModel vehicle = new SimpleVehicleViewModel()
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
                                TrunkSpace = reader.GetDecimal("trunkSpace"),
                                RentalCostPerDay = reader.GetDecimal("rentalCostPerDay"),
                                TimesRented = reader.GetInt32("timesRented")
                            };
                            vehicles.Add(vehicle);

                        }
                    }
                }
            }
            
            return View(vehicles);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
