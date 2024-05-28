using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace RentingSystem.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Database=vehicleDB;Uid=root;Pwd=;";

        // GET: Vehicles/Index
        public IActionResult Index()
        {
            List<Vehicle> vehicles = new List<Vehicle>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM vehicle";

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Create a new Vehicle object and populate its properties from the database
                            Vehicle vehicle = new Vehicle
                            {
                                VehicleID = reader.GetInt32("vehicleID"),
                                VehicleName = reader.GetString("vehicleName"),
                                Description = reader.GetString("description"),
                                LicensePlate = reader.GetString("licensePlate"),
                                FuelType = reader.GetString("fuelType"),
                                FuelTankCapacity = reader.GetFloat("fuelTankCapacity"),
                                FuelLevel = reader.GetFloat("fuelLevel"),
                                LicenseToOperate = reader.GetString("licenseToOperate")
                            };

                            // Add the vehicle to the list
                            vehicles.Add(vehicle);
                        }
                    }
                }
            }

            return View(vehicles); // Return the list of vehicles to the Index.cshtml view
        }
    }

    // Define the Vehicle model class
    public class Vehicle
    {
        public int VehicleID { get; set; }
        public string VehicleName { get; set; }
        public string Description { get; set; }
        public string LicensePlate { get; set; }
        public string FuelType { get; set; }
        public float FuelTankCapacity { get; set; }
        public float FuelLevel { get; set; }
        public string LicenseToOperate { get; set; }
    }
}

