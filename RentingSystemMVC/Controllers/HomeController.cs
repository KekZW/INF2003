using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using System.Windows.Input;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using NuGet.Protocol.Plugins;
using RentingSystemMVC.Data;
using RentingSystemMVC.Models;
using ThirdParty.Json.LitJson;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;

namespace RentingSystemMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly MongoDBContext _mongoContext;
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString = "Server=localhost;Database=vehicleDB;Uid=root;Pwd=;";

        public HomeController(ILogger<HomeController> logger, MongoDBContext mongoContext)
        {
            _logger = logger;
            _mongoContext = mongoContext;
        }

        public IActionResult Index()
        {
            
            List<SimpleVehicleViewModel> vehicles = new List<SimpleVehicleViewModel>();
            List<RentalHistory> rentalHistories = new List<RentalHistory>();

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

                // Fetch rental history and prepare data for MongoDB
                string rentalHistoryQuery =
                    "SELECT r.rentalID, r.vehicleID, r.userID, u.name, r.startRentalDate, r.endRentalDate " +
                    "FROM rental r " +
                    "JOIN user u ON u.userID = r.userID " +
                "WHERE r.endRentalDate < CURRENT_DATE";

                using (var rentalCommand = new MySqlCommand(rentalHistoryQuery, connection))
                {
                    using (var reader = rentalCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var history = new History
                            {
                                rentalID = reader.GetInt32("rentalID"),
                                userID = reader.GetInt32("userID"),
                                name = reader.GetString("name"),
                                startRentalDate = reader.GetDateTime("startRentalDate"),
                                endRentalDate = reader.GetDateTime("endRentalDate"),
                                status = "completed" // Assuming the status is "completed"
                            };

                            var rentalHistory = new RentalHistory
                            {
                                vehicleID = reader.GetInt32("vehicleID"),
                                History = new List<History> { history }
                            };

                            rentalHistories.Add(rentalHistory);
                        }
                    }
                }
            }
            foreach (var rentalHistory in rentalHistories)
            {
                var filter = Builders<RentalHistory>.Filter.Eq(r => r.vehicleID, rentalHistory.vehicleID);
                var update = Builders<RentalHistory>.Update
                    .AddToSetEach(r => r.History, rentalHistory.History);

                var updateOptions = new UpdateOptions { IsUpsert = true };
                _mongoContext.RentalHistory.UpdateOne(filter, update, updateOptions);
            }

            ViewBag.JsonData = JsonConvert.SerializeObject(rentalHistories);
            return View(vehicles);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
