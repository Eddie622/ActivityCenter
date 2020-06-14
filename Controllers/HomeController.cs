using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ActivityCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ActivityCenter.Controllers
{
    public class HomeController : Controller
    {
        private Context dbContext;

        public HomeController(Context context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);

                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                return RedirectToAction("Activities");
            }
            return View("Index");
        }

        [HttpPost]
        public IActionResult ProcessLogin(LoginUser user)
        {
            if (ModelState.IsValid)
            {
                User dbUser = dbContext.Users.FirstOrDefault(u => u.Email == user.LoginEmail);
                if (dbUser != null)
                {
                    PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                    if ((Hasher.VerifyHashedPassword(user, dbUser.Password, user.LoginPassword)) != 0)
                    {
                        HttpContext.Session.SetInt32("UserId", dbUser.UserId);
                        return RedirectToAction("Activities");
                    }
                }
            }
            ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
            return View("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Activities()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                ViewBag.LoggedUser = dbContext.Users
                    .FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));

                ViewBag.AllUsers = dbContext.Users
                    .ToList();

                var AllActivities = dbContext.PlannedActivitys
                    .Include(a => a.Participants)
                    .ThenInclude(u => u.User)
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.Time)
                    .ToList();

                return View(AllActivities);
            }
            return RedirectToAction("Index");
        }

        public IActionResult AddActivity()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                ViewBag.LoggedUser = dbContext.Users
                    .FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));

                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateActivity(PlannedActivity createdPlan)
        {
            if (ModelState.IsValid)
            {
                dbContext.PlannedActivitys.Add(createdPlan);
                dbContext.SaveChanges();

                Plan JoinedPlan = new Plan();
                JoinedPlan.PlannedActivityId = createdPlan.PlannedActivityId;
                JoinedPlan.UserId = (int)HttpContext.Session.GetInt32("UserId");
                dbContext.Plans.Add(JoinedPlan);
                dbContext.SaveChanges();

                return RedirectToAction("Activities");
            }

            ViewBag.LoggedUser = dbContext.Users
                .FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
            return View("AddActivity");
        }

        [HttpGet("Home/Activity/{ActivityId}/Join")]
        public IActionResult JoinActivity(int ActivityId)
        {
            PlannedActivity ThisActivity = dbContext.PlannedActivitys
                .FirstOrDefault(a => a.PlannedActivityId == ActivityId);

            Plan JoinedPlan = new Plan();
            JoinedPlan.PlannedActivityId = ThisActivity.PlannedActivityId;
            JoinedPlan.UserId = (int)HttpContext.Session.GetInt32("UserId");
            dbContext.Plans.Add(JoinedPlan);
            dbContext.SaveChanges();

            return RedirectToAction("Activities");
        }

        [HttpGet("Home/Activity/{ActivityId}/Leave")]
        public IActionResult LeaveActivity(int ActivityId)
        {
            PlannedActivity ThisActivity = dbContext.PlannedActivitys
                .FirstOrDefault(a => a.PlannedActivityId == ActivityId);

            Plan JoinedPlan = dbContext.Plans
                .FirstOrDefault(p => p.UserId == HttpContext.Session.GetInt32("UserId") && p.PlannedActivityId == ThisActivity.PlannedActivityId);

            dbContext.Plans.Remove(JoinedPlan);
            dbContext.SaveChanges();

            return RedirectToAction("Activities");
        }

        [HttpGet("Home/Activity/{ActivityId}/Delete")]
        public IActionResult DeleteActivity(int ActivityId)
        {
            PlannedActivity ThisActivity = dbContext.PlannedActivitys
                .FirstOrDefault(a => a.PlannedActivityId == ActivityId);

            dbContext.Remove(ThisActivity);
            dbContext.SaveChanges();

            return RedirectToAction("Activities");
        }

        [HttpGet("Home/Activity/{ActivityId}")]
        public IActionResult ActivityDetails(int ActivityId)
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                PlannedActivity ThisActivity = dbContext.PlannedActivitys
                    .Include(p => p.Participants)
                    .ThenInclude(u => u.User)
                    .FirstOrDefault(a => a.PlannedActivityId == ActivityId);

                ViewBag.Creator = dbContext.Users
                    .FirstOrDefault(user => user.UserId == ThisActivity.UserId);

                return View(ThisActivity);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
