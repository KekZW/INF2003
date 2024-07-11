using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RentingSystemMVC.Data;
using RentingSystemMVC.Models;

namespace RentingSystemMVC.Controllers
{
    public class SupportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MongoDBContext _mongoContext;

        public SupportController(ApplicationDbContext context, MongoDBContext mongoContext)
        {
            _context = context;
            _mongoContext = mongoContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string subject, string description)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                    Support newSupport = new Support
                    {
                        userID = userId,
                        status = "Open",
                        email = email,
                        creationDate = DateTime.Now,
                        updatedDate = DateTime.Now,
                        subject = subject,
                        description = description
                    };
                    _mongoContext.Support.InsertOne(newSupport);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return BadRequest(ModelState); // Return validation errors if ModelState is invalid
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                return StatusCode(500, "Internal Server Error");
            }
        }

        public IActionResult Manage()
        {
            var supports = _mongoContext.Support.Find(_ => true).ToList();
            return View(supports);
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            Console.WriteLine(id);
            var ticket = _mongoContext.Support.Find(t => t._id == id).FirstOrDefault();
            return View(ticket);
        }
    }
}