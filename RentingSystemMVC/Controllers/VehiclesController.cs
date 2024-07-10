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
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using System.Data;
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
        private readonly MongoDBContext _mongoContext;
        private static readonly FilterDefinitionBuilder<VehicleReview> filterBuilder = Builders<VehicleReview>.Filter;
        private static readonly FilterDefinitionBuilder<RentalHistory> filterHistoryBuilder = Builders<RentalHistory>.Filter;

        public VehiclesController(ApplicationDbContext context, MongoDBContext mongoContext)
        {
            _context = context;
            _mongoContext = mongoContext;
        }


        public IActionResult Index(DateTime? selectedDate, string? filterColumn, string? filterValue)
        {
            
            List<AuthorisedVehicleView> vehicles = new List<AuthorisedVehicleView>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand("GetVehicleInProgress", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("@todayDate", selectedDate.HasValue ? selectedDate : DateTime.Now.Date);
                    command.Parameters.AddWithValue("@filterColumn", filterColumn);
                    command.Parameters.AddWithValue("@filterValue", filterValue);

                   
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
                                TrunkSpace = reader.GetDecimal("trunkSpace"),
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
        public JsonResult CheckAvailability(int? rentalID, int vehicleID,  DateTime startRentalDate, DateTime endRentalDate)
        {
            bool available = true;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM rental " +
                               "WHERE vehicleID = @vehicleID " +
                               "AND (startRentalDate <= @endRentalDate AND endRentalDate >= @startRentalDate)";

                if (rentalID.HasValue)
                {
                    query += "AND rentalID != @rentalID ";
                }

                //Need change maintenanace workshopStatus value

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@vehicleID", vehicleID);
                    command.Parameters.AddWithValue("@startRentalDate", startRentalDate);
                    command.Parameters.AddWithValue("@endRentalDate", endRentalDate);

                    if (rentalID > 0)
                    {
                        command.Parameters.AddWithValue("@rentalID", rentalID);
                    }

                    int count = Convert.ToInt32(command.ExecuteScalar());
                           System.Diagnostics.Debug.WriteLine("Executing SQL Query: " + count);
                    if (count > 0)
                    {
                 
                        
                        available = false;
                    }
                }

                if (available)
                {
                    query = "SELECT COUNT(*) FROM maintenance " +
                            "WHERE vehicleID = @vehicleID " +   
                            "AND (startMaintDate > @endRentalDate AND startMaintDate > @startRentalDate) " +
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
                    Console.WriteLine("Count Result: " + ex.Message);
                    Console.WriteLine("Count Result: " + ex.Number);
                    Console.WriteLine("Count Result: " + ex.StackTrace);
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
                           "vt.seats, vt.fuelCapacity, vt.fuelType, vt.trunkSpace, vt.rentalCostPerDay " +
                           "FROM vehicle v " +
                           "INNER JOIN vehicleType vt ON v.vehicleTypeID = vt.vehicleTypeID ";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query +=
                    " WHERE (v.licensePlate LIKE @p0 OR vt.brand LIKE @p0 OR vt.model LIKE @p0 OR CONCAT(vt.brand, ' ', vt.model) LIKE @p0)";
            }

            query += "GROUP BY v.vehicleID";


            List <VehicleViewModel> vehicleList = _context.VehicleViewModel.FromSqlRaw(query, "%" + searchTerm + "%").ToList();
            return View(vehicleList);
        }
        
        public IActionResult Details(int id)
        {
  
            string vehQuery = "SELECT v.vehicleID, v.licensePlate, v.licenseToOperate, vt.brand, vt.model, vt.type, " +
                              "vt.seats, vt.fuelCapacity, vt.fuelType, vt.trunkSpace, vt.rentalCostPerDay " +
                              "FROM vehicle v " +
                              "INNER JOIN vehicleType vt ON v.vehicleTypeID = vt.vehicleTypeID " +
                              "WHERE v.vehicleID = @p0";

            VehicleViewModel vehicle = _context.VehicleViewModel
                .FromSqlRaw(vehQuery, id)
                .FirstOrDefault();

            List<Maintenance> maintenanceLogs = new List<Maintenance>();
            VehicleReview? vr = null;

            if (User.IsInRole("Admin")){
                
                // TODO: Retrieve maintenance logs for the vehicle, combine with vehicleViewModel 
                string maintenanceQuery = "SELECT * FROM maintenance WHERE vehicleID = @p0";

                if (_context.Maintenance != null)
                {
                    maintenanceLogs = _context.Maintenance.FromSqlRaw(maintenanceQuery, id).ToList();
                }
         
            } else{
                // Retrieve the reviews of the current vehicle ID
                var filter = filterBuilder.Eq("vehicleID",id);
                vr = _mongoContext.VehicleReview.Find(filter).FirstOrDefault();
            }
            
            VehicleDetailModel vehicleDetail = new VehicleDetailModel
                { Maintenances = maintenanceLogs, Vehicle = vehicle , vehicleReview = vr};
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
        [Authorize(Roles = "Admin")] 
        public IActionResult CreateVehicle(CreateVehicleViewModel model)
        {   
            
            if (ModelState.IsValid)
            {   

                try { 

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

                    return RedirectToAction("Manage");
                    
                } catch (Exception e){
                    ModelState.AddModelError(string.Empty, "There is existing license plate with these numbers, insert another number");
                    model.VehicleTypes = _context.VehicleType.ToList();
                    return View(model);
                }
                
            }
            return RedirectToAction("CreateVehicle");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public IActionResult CreateVehicleType() 
        {
            VehicleType vt = new VehicleType();
            ViewBag.Types = _context.VehicleType.Select(v => v.Type).Distinct().ToList(); 
            return View(vt); 
        } 

        [HttpPost] 
        [Authorize(Roles = "Admin")]
        public IActionResult CreateVehicleType(VehicleType vt) 
        { 
 
            if (ModelState.IsValid) 
            { 

                try { 
                    using (var connection = new MySqlConnection(_connectionString)) 
                    { 
                        connection.Open(); 
                        string query = "INSERT INTO vehicletype (brand, model, type, seats, fuelCapacity, fuelType, trunkSpace, rentalCostPerDay) " + 
                                    "VALUES (@Brand, @Model, @Type, @Seats, @FuelCapacity, @FuelType, @TrunkSpace, @RentalCostPerDay)"; 
    
                        using (var command = new MySqlCommand(query, connection)) 
                        { 
                            command.Parameters.AddWithValue("@Brand", vt.Brand); 
                            command.Parameters.AddWithValue("@Model", vt.Model); 
                            command.Parameters.AddWithValue("@Type", vt.Type); 
                            command.Parameters.AddWithValue("@Seats", vt.Seats); 
                            command.Parameters.AddWithValue("@FuelCapacity", vt.FuelCapacity); 
                            command.Parameters.AddWithValue("@FuelType", vt.FuelType); 
                            command.Parameters.AddWithValue("@TrunkSpace", vt.TrunkSpace); 
                            command.Parameters.AddWithValue("@RentalCostPerDay", vt.RentalCostPerDay); 
    
                            command.ExecuteNonQuery(); 
                        } 
                    } 
 
                    return RedirectToAction("CreateVehicle"); 

                } catch (Exception e){
                    return View(vt);
                }
            } 
    
            return RedirectToAction("CreateVehicle"); 
        }



        [HttpPost]
        public IActionResult AddMaintenance(int vehicleID, DateTime startDate, DateTime endDate, string workshopStatus)
        {
            using(var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO maintenance (vehicleID, workshopStatus, startMaintDate, endMaintDate) "
                + "VALUES (@vehicleID, @workshopStatus, @startDate, @endDate)";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@vehicleID", vehicleID);
                    command.Parameters.AddWithValue("@workshopStatus", workshopStatus);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);

                    command.ExecuteNonQuery();
                }
            }
            
            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public IActionResult DeleteVehicle(int vehicleID)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try {
                    connection.Open();

                    string query = "CALL Safe_Drop_Vehicle(@vehicleID)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@vehicleID", vehicleID);

                        command.ExecuteNonQuery();
                    }

                }catch (Exception e){
                    return Json(new { success = false });
                }
            }
            return Json(new { success = true });
        }   
        [HttpPost]
        public async Task<IActionResult> EditMaintenance(Maintenance maintenance)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "UPDATE maintenance SET startMaintDate = @StartDate, endMaintDate = @EndDate, workshopStatus = @WorkshopStatus " +
                                "WHERE maintenanceID = @MaintenanceID";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaintenanceID", maintenance.MaintenanceID);
                        command.Parameters.AddWithValue("@StartDate", maintenance.startMaintDate.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@EndDate", maintenance.endMaintDate.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@WorkshopStatus", maintenance.WorkshopStatus);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }
    
        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult postReview(int vehicleId, string comment, int rating)
        {
            try
            {

                var filter = Builders<VehicleReview>.Filter.And(
                    Builders<VehicleReview>.Filter.Eq("vehicleID", vehicleId),
                    Builders<VehicleReview>.Filter.ElemMatch("reviews", Builders<Review>.Filter.Eq("name", User.FindFirst(ClaimTypes.Name)?.Value))
                );

                var existingUserReview = _mongoContext.VehicleReview.Find(filter).FirstOrDefault();
       
               if(existingUserReview != null) return BadRequest();

                var update = Builders<VehicleReview>.Update.Push("reviews", new Review
                {
                    name = User.FindFirst(ClaimTypes.Name)?.Value,
                    rating = rating,
                    comment = comment
                });

                var options = new FindOneAndUpdateOptions<VehicleReview>{
                    ReturnDocument = ReturnDocument.After,
                    IsUpsert = true,

                };
               
                _mongoContext.VehicleReview.FindOneAndUpdate(filter, update,options);

                return Json (new {success = true});    

            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpPost]
        [Authorize(Roles = "User")] 
        public ActionResult DeleteReview(int vehicleID){
            try {
                
                var filter = filterBuilder.And(
                    filterBuilder.Eq("vehicleID", vehicleID),
                    filterBuilder.ElemMatch("reviews", Builders<Review>.Filter.Eq("name", User.FindFirst(ClaimTypes.Name)?.Value))
                );
            
                var update =  Builders<VehicleReview>.Update.PullFilter("reviews", Builders<Review>.Filter.Eq("name", User.FindFirst(ClaimTypes.Name)?.Value));
                _mongoContext.VehicleReview.UpdateOne(filter, update);

                return  Json( new {success = true});
            } catch (Exception ex) {
                return StatusCode (500, "Internal Server error: " + ex.Message);
            } 
        
        }

        [HttpGet]
        [Authorize(Roles="Admin")]
        public ActionResult getAllRentalHistory(int vehicleID,int pageNumber, int pageSize){

            var filter = filterHistoryBuilder.Eq("vehicleID", vehicleID);
            var rentalHistoryData = _mongoContext.RentalHistory.Find(filter).FirstOrDefault();

            if (rentalHistoryData == null){
                return Json(new { success = false, message = "Car not found" });
            }

            var totalItemsCount = rentalHistoryData.History?.Count() ?? 0;
              var paginatedTotalItems = rentalHistoryData.History
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();
                
            var result = new {
                success = true,
                paginatedTotalItems = paginatedTotalItems,
                totalItemsCount = totalItemsCount,
                pageNumber = pageNumber,
                pageSize = pageSize
            };

            return Json(result);

        }

    }   
    
}
