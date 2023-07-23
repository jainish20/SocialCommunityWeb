using communityWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;

namespace communityWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProjectContext projDbContext;

        public AdminController(ProjectContext p)
        {
            this.projDbContext = p;
        }
        // GET: AdminController
        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("aname") == null)
            {
                return View("login", "admin");
            }
            else
            {

            return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Verify(string name, string password)
        {
            var admin = projDbContext.Admins.Where(u => u.UName == name).FirstOrDefault();
            if (admin != null)
            {
                if (admin.IsActive == false)
                {
                    HttpContext.Session.SetString("awrong", "you are no longer active please go for your activation process");
                    return RedirectToAction("login", "admin");
                }
                else
                {

                    var storedHashedPassword = admin.Password;
                    var isPasswordValid = BCryptNet.Verify(password, storedHashedPassword);

                    if (isPasswordValid)
                    {
                        Console.WriteLine("valid");
                        HttpContext.Session.SetString("aname", admin.UName);
                        HttpContext.Session.SetInt32("aId", admin.Id);
                        HttpContext.Session.Remove("awrong");




                        return RedirectToAction("Index", "admin");

                        /*                    return RedirectToAction("index", "home");
                        */                    // Password is valid, proceed with login
                    }
                    else
                    {
                        HttpContext.Session.SetString("awrong", "The user name or password you've entered is incorrect");
                        ViewBag.awrong = "The email or password you've entered is incorrect";
                        return RedirectToAction("login", "admin");
                        // Password is invalid, handle authentication failure
                    }

                }

            }
            else
            {

                HttpContext.Session.SetString("awrong", "The user name or password you've entered is incorrect");
                ViewBag.awrong = "The email or password you've entered is incorrect";
                return RedirectToAction("login", "admin");
            }

            // Verify the entered password against the stored hash


           
        }
        public ActionResult logout()
        {
            HttpContext.Session.Remove("aname");
            HttpContext.Session.Remove("aId");
            return View("login");
        }
        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
