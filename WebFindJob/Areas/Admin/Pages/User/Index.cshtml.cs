using WebFindJob.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static WebFindJob.Areas.Admin.Pages.User.IndexModel;
using Microsoft.AspNetCore.Authorization;
using WebFindJob.Data;
namespace WebFindJob.Areas.Admin.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context=context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public List<UserRoleViewModel> UsersWithRoles { get; set; }

        public async Task OnGetAsync()
        {
            UsersWithRoles = new List<UserRoleViewModel>();
            var users = await _userManager.Users.ToListAsync();
            var seekers = await _context.Seeker.ToListAsync();
            var recruiter= await _context.Recruiter.ToListAsync();
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var roleNames = string.Join(", ", userRoles);

                var userSeeker = seekers.FirstOrDefault(s => s.Id == user.Id); 
                var userRecruiter = recruiter.FirstOrDefault(s => s.Id == user.Id);

                UsersWithRoles.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Seeker = userSeeker != null ? userSeeker.Id : "N/A",
                    Recruiter = userRecruiter != null ? userRecruiter.Id : "N/A",
                    RoleNames = roleNames
                });
            }
        }

        public class UserRoleViewModel
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string RoleNames { get; set; }
            public string Seeker { get; set; }
            public string Recruiter { get; set; }
        }
    }

}
