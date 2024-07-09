using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentingSystemMVC.Data;
using RentingSystemMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.ComponentModel.DataAnnotations;



namespace RentingSystemMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // TODO: Login Functionality (Cookie-based authentication)
            // 1. Retrieve user from database using WHERE statement (done)
            // 2. If user exists, retrieve respective password (done)
            // 3. Compare hash inputted password with retrieved hashed password from database (done)
            // 4. If correct, redirect to Home/Index, create authentication cookie

            string query = "SELECT * FROM user WHERE emailAddress = @p0 AND role IN ('User','Admin')";

            var user = _context.User.FromSqlRaw(query, email).FirstOrDefault();

            if (user != null)
            {
                byte[] salt = new byte[0];

                string hashed_pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8
                ));
                if (user.UserPassword == hashed_pass)
                {   
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.EmailAddress),
                        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                        new Claim(ClaimTypes.Name, user.name),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    
                    if ( user.Role == "User"){
                        return RedirectToAction("Index", "Vehicles");
                    }

                    return RedirectToAction("Manage", "Vehicles");
                }
            } 
            
            ModelState.AddModelError(string.Empty, "Unassociated account, Please Register and Login");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(
            string firstName,
            string lastName,
            string username,
            string email,
            string phoneNumber,
            string address,
            string licenseClass,
            DateTime licenseDate,
            string password,
            string confirmPassword
        )
        {
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                ModelState.AddModelError("email", "Invalid email format.");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return View();
            }

            byte[] salt = new byte[0];

            string hashed_pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            ));
            
            var user = new User
            {
                UserPassword = hashed_pass,
                name = firstName + " " + lastName,
                Address = address,
                EmailAddress = email,
                PhoneNo = phoneNumber,
            };

            try {

                _context.User.Add(user);
                _context.SaveChanges();

            } catch (Exception e ) {

                ModelState.AddModelError(string.Empty, "An error occurred while creating the user account. Please try again later.");
                return View();

            } 

            int userId = user.UserID;

            var License = new License
            {
                AcquireDate = licenseDate,
                LicenseClass = licenseClass,
                UserID = userId
            };
            
            _context.License.Add(License);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }
        
        [Authorize(Roles="Admin,User")]
        public async Task<IActionResult> Logout()
        {
            // Clear the existing external cookie
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles="Admin,User")]
        [HttpGet]
        public IActionResult Information()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Information(String oldPass, String newPass)
        {
            int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            byte[] salt = new byte[0];

            if (oldPass == newPass){
                ViewData["Message"] = "Password cannot be same.";
                return View();
            }

            string hashed_old_pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: oldPass,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            ));

            String hashed_database_old_pass = _context.User.Where(x => x.UserID == userId)
                .Select(x => x.UserPassword)
                .FirstOrDefault();


            if (hashed_old_pass != hashed_database_old_pass){
                ViewData["Message"] = "Password does not match";
                return View();
            }

            string hashed_new_pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: newPass,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            ));

            var user = _context.User.FirstOrDefault(x => x.UserID == userId);
            user.UserPassword = hashed_new_pass;
            _context.SaveChanges();
           
            ViewData["Message"] = "Successfully updated password";
            return View();
        }

        [Authorize (Roles = "User")]
        [HttpPost]
        public IActionResult closeAccount(){

            int userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int? rentalID = _context.Rental.Where(x => x.UserID == userId && x.endRentalDate >= DateTime.Now).Select(x => x.rentalID).FirstOrDefault();

            if (rentalID != 0){
                ViewData["Message"] = "You have rental booking, you cant remove your account";
                return View("Information");
            }

            var userObj = _context.User.FirstOrDefault(x => x.UserID == userId);
            userObj.Role = "cls";

            _context.SaveChanges();

            Logout();

            return Json(new { success = true });
        }
    }
}