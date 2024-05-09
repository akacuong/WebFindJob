using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using WebFindJob.Data;
using WebFindJob.Models;

namespace WebFindJob.Controllers
{
    public class RecruitersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RecruitersController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Recruiters
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(await _context.Recruiter.ToListAsync());
            }
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound();
            }

            var Recruiters = await _context.Recruiter
                .Where(j => j.Id == currentUser.Id)
                .ToListAsync();
            ViewBag.Id = currentUser.Id;
            return View(Recruiters);
        }

        // GET: Recruiters/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recruiter = await _context.Recruiter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recruiter == null)
            {
                return NotFound();
            }

            return View(recruiter);
        }

        // GET: Recruiters/Create
        [Authorize(Roles = "Admin,Recruiter")]
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

        // POST: Recruiters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyName,Email,PhoneNumber,Address,Description")] Recruiter recruiter)
        {
            var users = _userManager.Users.ToList();
            if (ModelState.IsValid)
            {
                // Kiểm tra xem ID đã tồn tại trong bảng Seeker hoặc Recruiter chưa
                bool isIdExistsInSeeker = await _context.Seeker.AnyAsync(s => s.Id == recruiter.Id);
                bool isIdExistsInRecruiter = await _context.Recruiter.AnyAsync(r => r.Id == recruiter.Id);
                if (isIdExistsInSeeker || isIdExistsInRecruiter)
                {
                    ModelState.AddModelError(string.Empty, $"Id={recruiter.Id} have already exists");
                    ViewBag.Users = new SelectList(users, "Id", "Id", recruiter.Id); // Gửi lại danh sách ID với ID đã chọn trước đó
                    return View(recruiter);
                }
                // Tiếp tục xử lý nếu ID không tồn tại hoặc chỉ tồn tại trong một bảng
            }
            ViewBag.Users = new SelectList(users, "Id", "Id", recruiter.Id);
            _context.Add(recruiter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Recruiters/Edit/5
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recruiter = await _context.Recruiter.FindAsync(id);
            if (recruiter == null)
            {
                return NotFound();
            }
            return View(recruiter);
        }

        // POST: Recruiters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,CompanyName,Email,PhoneNumber,Address,Description")] Recruiter recruiter)
        {
            if (id != recruiter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recruiter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecruiterExists(recruiter.Id))
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
            return View(recruiter);
        }

        // GET: Recruiters/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recruiter = await _context.Recruiter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recruiter == null)
            {
                return NotFound();
            }

            return View(recruiter);
        }
        [Authorize(Roles = "Admin")]
        // POST: Recruiters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var recruiter = await _context.Recruiter.FindAsync(id);
            if (recruiter == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                // Xóa Seeker
                _context.Recruiter.Remove(recruiter);
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

        private bool RecruiterExists(string id)
        {
            return _context.Recruiter.Any(e => e.Id == id);
        }
    }
}
