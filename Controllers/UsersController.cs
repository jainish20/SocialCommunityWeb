using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using communityWeb.Models;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using PagedList;
using BCryptNet = BCrypt.Net.BCrypt;


namespace communityWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly ProjectContext _context;
        private readonly IWebHostEnvironment hostEnvironment;

        public UsersController(ProjectContext context,IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
        }

        // GET: Users
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            var projectContext = _context.Users.ToList();
            if (searchString != null)
            {
                projectContext = projectContext.Where(a => a.Fname.ToLower().Contains(searchString)||a.Lname.ToLower().Contains(searchString)||a.EmailId.ToLower().Contains(searchString)||a.ContactNo.Contains(searchString)||a.City.ToLower().Contains(searchString)||a.State.ToLower().Contains(searchString)).ToList();
            }
            int pageSize = 10;
            ViewBag.pageSize = pageSize;
            if (page <= 0)
            {
                page = 1;
            }
            int pageNumber = (page ?? 1);
            ViewBag.page = pageNumber;
            int totalItems = projectContext.Count();
            ViewData["totalItems"] = totalItems;
            float b = totalItems / pageSize;
            if (totalItems % pageSize == 0)
            {

                ViewBag.totalPage = b;

            }
            else
            {

                ViewBag.totalPage = b + 1;
            }

            var pagedProducts = new StaticPagedList<User>(
                projectContext.Skip((pageNumber - 1) * pageSize).Take(pageSize),
                pageNumber,
                pageSize,
                totalItems);
            ViewBag.CurrentFilter = searchString;
            return View(pagedProducts);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fname,Lname,EmailId,Password,Address,City,State,ContactNo,IsActive,Gender")] User user,IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                user.RegisteredDate = DateTime.Now;
                var hasPass = BCryptNet.HashPassword(user.Password);
                user.Password = hasPass;
                if (photo != null)
                {
                    string filename = photo.FileName;
                    string filepath = Path.Combine(hostEnvironment.WebRootPath, "img", filename);

                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {

                        await photo.CopyToAsync(stream);
                    }
                    user.Image = filename;

                }
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fname,Lname,EmailId,Password,Address,City,State,ContactNo,IsActive,Gender")] User user,IFormFile photo)
        {
            
            if (id != user.Id)
            {
                return NotFound();
            }

           
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);

                    // Update the fields of the retrieved record with the values from the user parameter
                    existingUser.Fname = user.Fname;
                    existingUser.Lname = user.Lname;
                    existingUser.EmailId = user.EmailId;
                    var hasPass = BCryptNet.HashPassword(user.Password);
                   
                    existingUser.Password = hasPass;
                    existingUser.Address = user.Address;
                    existingUser.City = user.City;
                    existingUser.State = user.State;
                    existingUser.ContactNo = user.ContactNo;
                    existingUser.IsActive = user.IsActive;
                    existingUser.Gender = user.Gender;
                    if (photo != null)
                    {
                        string filename = photo.FileName;
                        string filepath = Path.Combine(hostEnvironment.WebRootPath, "img", filename);

                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {

                            await photo.CopyToAsync(stream);
                        }
                        existingUser.Image = filename;

                    }
                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ProjectContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return _context.Users.Any(e => e.Id == id);
        }
    }
}
