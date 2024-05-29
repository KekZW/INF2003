using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using RentingSystemMVC.Models;

namespace RentingSystem.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Database=vehicleDB;Uid=root;Pwd=;";

        // GET: Vehicles/Index
        public IActionResult Index(string filterColumn, string filterValue)
        {
            List<Vehicle> vehicles = new List<Vehicle>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT v.vehicleID, v.licensePlate, v.licenseToOperate, vt.brand, vt.model, vt.type," +
                               " vt.seats, vt.fuelCapacity, vt.fuelType, vt.truckSpace, vt.rentalCostPerDay FROM vehicle v " +
                               "INNER JOIN vehicleType vt ON v.vehicleTypeID = vt.vehicleTypeID";

                if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterValue))
                {
                    query += " WHERE " + filterColumn + " LIKE @filterValue";
                }

                using (var command = new MySqlCommand(query, connection))
                {
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
                                TruckSpace = reader.GetString("truckSpace"),
                                RentalCostPerDay = reader.GetDecimal("rentalCostPerDay")
                            };

                            vehicles.Add(vehicle);
                        }
                    }
                }
            }

            return View(vehicles);
        }
    }
}
