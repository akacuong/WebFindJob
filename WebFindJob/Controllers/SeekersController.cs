using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using WebFindJob.Data;
using WebFindJob.Models;

namespace WebFindJob.Controllers
{
    public class SeekersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SeekersController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Seekers
        [Authorize(Roles = "Admin,Seeker")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(await _context.Seeker.ToListAsync());
            }
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound();
            }

            var Seekers = await _context.Seeker
                .Where(j => j.Id == currentUser.Id)
                .ToListAsync();
            ViewBag.Id = currentUser.Id;
            return View(Seekers);
        }

        // GET: Seekers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seeker = await _context.Seeker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seeker == null)
            {
                return NotFound();
            }

            return View(seeker);
        }

 
        [Authorize(Roles = "Admin,Seeker")]
        public IActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                var users = _userManager.Users.ToList();
                ViewBag.Users = new SelectList(users, "Id", "Id");
            }
            else
            {
                var currentUser = _userManager.GetUserAsync(User).Result;
                ViewBag.Users = new SelectList(new List<IdentityUser> { currentUser }, "Id", "Id");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,PhoneNumber,Address,Email,Description")] Seeker seeker)
        {
            var users = _userManager.Users.ToList();
            if (ModelState.IsValid)
            {
                bool isIdExistsInSeeker = await _context.Seeker.AnyAsync(s => s.Id == seeker.Id);
                bool isIdExistsInRecruiter = await _context.Recruiter.AnyAsync(r => r.Id == seeker.Id);
                if (isIdExistsInSeeker || isIdExistsInRecruiter)
                {
                    ModelState.AddModelError(string.Empty, $"Id={seeker.Id} have already exists");
                    ViewBag.Users = new SelectList(users, "Id", "Id", seeker.Id);
                    return View(seeker);
                }
            }
            ViewBag.Users = new SelectList(users, "Id", "Id", seeker.Id);
            _context.Add(seeker);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seeker = await _context.Seeker.FindAsync(id);
            if (seeker == null)
            {
                return NotFound();
            }
            return View(seeker);
        }

        // POST: Seekers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,PhoneNumber,Address,Email,Description")] Seeker seeker)
        {
            if (id != seeker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seeker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeekerExists(seeker.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(seeker);
        }

        // GET: Seekers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seeker = await _context.Seeker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seeker == null)
            {
                return NotFound();
            }

            return View(seeker);
        }

        // POST: Seekers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var seeker = await _context.Seeker.FindAsync(id);
            if (seeker == null)
            {
                return NotFound();
            }

            // Tìm user tương ứng
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                // Xóa Seeker
                _context.Seeker.Remove(seeker);
                // Xóa User
                await _userManager.DeleteAsync(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return RedirectToAction("Error", "Home");
            }
        }

        private bool SeekerExists(string id)
        {
            return _context.Seeker.Any(e => e.Id == id);
        }
    }
}
