using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
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
                        description = description,
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
                Console.WriteLine(ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        public IActionResult Manage()
        {
            int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var filter = Builders<Support>.Filter.Eq("assigned_to", userId);
            var supports = _mongoContext.Support.Find(filter).SortBy(Support => Support.status).ToList();
            return View(supports);
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            Console.WriteLine(id);
            TempData["TicketId"] = id;
            var ticket = _mongoContext.Support.Find(t => t._id == id).FirstOrDefault();

            if (ticket == null)
            {
                return NotFound();
            }

            string query = "SELECT * FROM user WHERE userID = @p0 AND role IN ('User','Admin')";
            User user = _context.User.FromSqlRaw(query, ticket.userID).FirstOrDefault();
            ticket.user = user;
            
            if (ticket.comments != null)
            {
                foreach (var comment in ticket.comments)
                {
                    Console.WriteLine(comment.user_id);
                    string commentQuery = "SELECT * FROM user WHERE userID = @p0 AND role IN ('User','Admin')";
                    User commentUser = _context.User.FromSqlRaw(commentQuery, comment.user_id).FirstOrDefault();
                    comment.user = commentUser;
                }
            }

            return View(ticket);
        }

        [HttpPost]
        public IActionResult Comment(string comment)
        {
            // Console.WriteLine(comment);
            string id = TempData["TicketId"] as string;
            
            if (string.IsNullOrEmpty(id))
            {
                // Handle the case where id is null or empty
                return BadRequest("Ticket ID is missing");
            }
            
            int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var filter = Builders<Support>.Filter.Eq("_id", id);
            var update = Builders<Support>.Update.Push("comments", new Comment
            {
                comment_id = ObjectId.GenerateNewId(),
                comment_date = DateTime.Now,
                user_id = userId,
                comment_text = comment
            })
            .Set("updatedDate", DateTime.Now);
            
            _mongoContext.Support.UpdateOne(filter, update);
            
            // return Json(data);

            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult Resolve()
        {
            string id = TempData["TicketId"] as string;
            
            if (string.IsNullOrEmpty(id))
            {
                // Handle the case where id is null or empty
                return BadRequest("Ticket ID is missing");
            }
            
            var filter = Builders<Support>.Filter.Eq("_id", id);
            var update = Builders<Support>.Update.Set("status", "Resolved");
            _mongoContext.Support.UpdateOne(filter, update);
            
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult Assign()
        {
            var filter = Builders<Support>.Filter.Eq("assigned_to", BsonNull.Value);
            var unassignedSupports = _mongoContext.Support.Find(filter).ToList();

            
            // var supports = _mongoContext.Support.Find(_ => true).ToList();
            return View(unassignedSupports);
        }

        [HttpPost]
        public IActionResult Assign(List<string> selectedTickets)
        {
            int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            foreach(var ticketId in selectedTickets)
            {
                var filter = Builders<Support>.Filter.Eq("_id", ticketId);
                var update = Builders<Support>.Update.Set("assigned_to", userId);
                _mongoContext.Support.UpdateOne(filter, update);
            }
            return RedirectToAction("Manage");
        }
    }
}