using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; //this is where session comes from
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WeddingPlanner.Models;


namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context { get; }
        public HomeController(MyContext context)
        {
            _context = context;
        }


        // VIEW (Wedding)
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {

            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }
            else
            {

                ViewBag.AllWeddings = _context.Weddings
                .Include(wedding => wedding.Rsvp)
                    .OrderBy(wedding => wedding.WedderOne)
                    .ToList();

                ViewBag.LoggedInUser = Convert.ToInt16(UserId);

                return View();
            }
        }

        [HttpGet("wedding/new")]
        public IActionResult NewWedding()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }


        // CREATE (Wedding)
        [HttpPost("wedding/create")]
        public IActionResult CreateWedding(Wedding fromForm)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                fromForm.CreatorId = Convert.ToInt16(UserId);
                _context.Add(fromForm);
                _context.SaveChanges();

                System.Console.WriteLine(fromForm.WeddingId);

                return RedirectToAction("ViewWedding", new { weddingId = fromForm.WeddingId });
            }
            else
            {
                return View("NewWedding");
            }
        }

        // RENDER ONE (wedding)
        [HttpGet("wedding/{weddingId}")]
        public IActionResult ViewWedding(int weddingId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ThisWedding = _context.Weddings
                    .Include(wedding => wedding.Rsvp)
                        .ThenInclude(rsvp => rsvp.RsvpUser)
                .FirstOrDefault(wedding => wedding.WeddingId == weddingId);

                ViewBag.AllGuests = _context.Users
                    .Include(guest => guest.Rsvp)
                        .ThenInclude(rsvp => rsvp.RsvpWedding)
                    .Where(wedding => wedding.Rsvp.Any(rsvp => rsvp.RsvpWeddingId == weddingId))
                    // .Where(wedding => !wedding.Rsvp.Any(rsvp => rsvp.RsvpWeddingId == weddingId)) //would be this if you wanted a list of guests NOT going(the "!" is the difference)
                    .ToList();

                return View();
            }
        }

        // UPDATE
        [HttpGet("edit/{weddingId}")]
        public IActionResult EditWedding(int weddingId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }

            Wedding ToEditWedding = _context.Weddings.FirstOrDefault(wedding => wedding.WeddingId == weddingId);

            if(ToEditWedding == null || ToEditWedding.CreatorId != (int)UserId) //ChangeTrackingStrategy for null OR if the user signed-in created the wedding they are trying to edit.
            {
                return RedirectToAction("Dashboard");
            }

            return View("EditWedding", ToEditWedding);
        }

        [HttpPost("update/{weddingId}")]
        public IActionResult UpdateWedding(int weddingId, Wedding fromForm)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }

            Wedding ToEdit = _context.Weddings.FirstOrDefault(wedding => wedding.WeddingId == weddingId);

            if(ToEdit.CreatorId != (int)UserId) //did the logged-in user make this?
            {
                return RedirectToAction("Dashboard", new {weddingId = weddingId});
            }
            else if(ModelState.IsValid)
            {
                Wedding inDb = _context.Weddings.FirstOrDefault(wedding => wedding.WeddingId == weddingId);

                inDb.WedderOne = fromForm.WedderOne;
                inDb.WedderTwo = fromForm.WedderTwo;
                inDb.Date = fromForm.Date;
                inDb.Address = fromForm.Address;
                inDb.UpdatedAt = fromForm.UpdatedAt;

                _context.SaveChanges();

                return RedirectToAction("ViewWedding", new {weddingId = weddingId});
            }
            else
            {
                return EditWedding(weddingId);
            }

        }







        // RSVP -------------------------------------------------------------------
        [HttpGet("rsvp/{weddingId}")]
        public IActionResult RsvpToWedding(int weddingId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }

            Rsvp ToAddRsvp = new Rsvp
            {
                RsvpUserId = (int)HttpContext.Session.GetInt32("UserId"),
                RsvpWeddingId = weddingId
            };

            _context.Add(ToAddRsvp);
            _context.SaveChanges();

            System.Console.WriteLine(ToAddRsvp.RsvpId);

            return RedirectToAction("Dashboard", new { rsvpId = ToAddRsvp.RsvpId });
        }

        // RSVP DELETE
        [HttpGet("rsvp/delete/{weddingId}/{userId}")]
        public RedirectToActionResult DeleteRsvp(int weddingId, int userId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }

            Rsvp ToDeleteRsvp = _context.Rsvps.FirstOrDefault(rsvp => rsvp.RsvpWeddingId == weddingId && rsvp.RsvpUserId == userId);

            if (userId != ToDeleteRsvp.RsvpUserId)
            {
                return RedirectToAction("Dashboard");
            }
            else
            {
                _context.Remove(ToDeleteRsvp);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }





        // LOGIN/REGISTRATION -------------------------------------

        // VIEW(log/reg)
        [HttpGet("")]
        public ViewResult Index()
        {
            return View();
        }

        // CREATE (Register)
        [HttpPost("register/user")]
        public IActionResult Register(User fromForm)
        {
            if (ModelState.IsValid)
            {
                // Checking if email is already registered
                if (_context.Users.Any(user => user.Email == fromForm.Email))
                {
                    // If it is already registered, back to main/dashboard/whatever
                    return RedirectToAction("Index");
                }

                // otherwise, encrypt the password
                PasswordHasher<User> hasher = new PasswordHasher<User>();

                fromForm.Password = hasher.HashPassword(fromForm, fromForm.Password);

                _context.Add(fromForm);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", fromForm.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Register");
            }
        }

        // RENDER (Login)
        [HttpPost("login/user")]
        public IActionResult Login(LoginUser fromForm)
        {
            if (ModelState.IsValid)
            {
                User inDb = _context.Users.FirstOrDefault(user => user.Email == fromForm.LogEmail);

                if (inDb == null)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Email/Password");
                    return RedirectToAction("Index");
                }

                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

                var result = hasher.VerifyHashedPassword(fromForm, inDb.Password, fromForm.LogPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("LogEmail", "Invalid Email/Password");
                    return RedirectToAction("Index");
                }

                HttpContext.Session.SetInt32("UserId", inDb.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Login");
            }
        }

        // LOGOUT
        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // DELETE
        [HttpGet("{weddingId}/delete")]
        public RedirectToActionResult DeleteWedding(int weddingId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }

            Wedding toDelete = _context.Weddings.FirstOrDefault(wedding => wedding.WeddingId == weddingId);

            if ((int)UserId != (int)toDelete.CreatorId)
            {
                return RedirectToAction("Dashboard");
            }

            _context.Remove(toDelete);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}