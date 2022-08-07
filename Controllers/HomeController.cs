using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;



namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            ViewBag.Weddings = _context.Weddings
                .Include(w => w.Attendees)
                .ThenInclude(a => a.Attendee);
            // handle loggedIn..
            if(userId != null)
            {
                ViewBag.LoggedIn = true;
                ViewBag.userId = userId;
                return View();
            } else if(userId == null)
            {
                ViewBag.LoggedIn = false;
                ViewBag.userId = userId;

                return View();
            }
            ViewBag.LoggedIn = false;
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!"); 
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                user.Balance = 500;
                _context.Users.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("userId", user.UserId);
                return RedirectToAction("Success");
            }
            return View("Index");
        }
        [HttpGet("login/form")]
        public IActionResult LoginForm()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if(userId != null){
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost("login")]
        public IActionResult LoginUser(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                int minus = -100;
                Console.WriteLine(minus);
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("LoginForm");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password); 
                if(result == 0)
                {
                    // if didn't pass
                    ModelState.AddModelError("Password", "Invalid Password");
                    return View("LoginForm");
                }
                // if logged in successfully
                // store userId in session
                HttpContext.Session.SetInt32("userId", userInDb.UserId);
                return RedirectToAction("Index");
            }
            return View("LoginForm");
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if(userId != null){
                return View();
            }
            return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            Console.WriteLine(userId);
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("planner")]
        public IActionResult Planner()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if(userId != null){
                ViewBag.UserId = userId;
                return View();
            }
            return View("Index");
        }

        [HttpPost("wedding/create")]
        public IActionResult CreateWedding(Wedding newWedding)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            // associate creatorId
            // add wedding to user.MyWeddings
            if(ModelState.IsValid)
            {
                _context.Weddings.Add(newWedding);
                User user = _context.Users
                .Include(u => u.MyWeddings)
                .SingleOrDefault(u => u.UserId == userId);
                user.MyWeddings.Add(newWedding);
                
                _context.SaveChanges();
                return RedirectToAction("WeddingDetails", new {weddingId = newWedding.WeddingId});
            }
            // return the planner form with errors
            if(userId != null){
                ViewBag.UserId = userId;
                return View("Planner");
            }
            return View("Index");
        }
        [HttpPost("wedding/delete")]
        public IActionResult DeleteWedding(int UserId, int WeddingId)
        {
            Wedding wedding = _context.Weddings
                .SingleOrDefault(w => w.WeddingId == WeddingId);
            _context.Weddings.Remove(wedding);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost("rsvp")]
        public IActionResult RSVP(int UserId, int WeddingId)
        {
            AttendRelation newRelation = new AttendRelation();
            newRelation.UserId = UserId;
            newRelation.WeddingId = WeddingId;
            
            _context.AttendRelations.Add(newRelation);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost("unrsvp")]
        public IActionResult UNRSVP(int UserId, int WeddingId)
        {
            // delete relation
            AttendRelation relation = _context.AttendRelations
                .SingleOrDefault(r => r.WeddingId == WeddingId && r.UserId == UserId);
            _context.AttendRelations.Remove(relation);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet("wedding/details/{weddingId}")]
        public IActionResult WeddingDetails(int weddingId)
        {
            Wedding wedding = _context.Weddings
            .Include(w => w.Attendees)
                .ThenInclude(a => a.Attendee)
            .SingleOrDefault(w => w.WeddingId == weddingId);
            return View(wedding);
        }
    }
}
