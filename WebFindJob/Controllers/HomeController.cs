using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebFindJob.Data;
using WebFindJob.Models;

namespace WebFindJob.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            var jobListings = _context.JobListing.ToList();
            return View(jobListings);
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
        [HttpGet("Index")]
        public IActionResult Index(string searchString)
        {
            var jobListings = _context.JobListing.ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                jobListings = jobListings.Where(j => j.Title.Contains(searchString) || j.Description.Contains(searchString)).ToList();
            }

            return View(jobListings);
        }
    }
}
