using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string id = _userManager.GetUserId(HttpContext.User);
            IQueryable<Ticket> userTickets;
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            userTickets = user.Tickets.AsQueryable();
            ViewData["LowCount"] = userTickets.Where(x => x.Priority == "Low").ToList().Count();
            ViewData["MediumCount"] = userTickets.Where(x => x.Priority == "Medium").ToList().Count();
            ViewData["HighCount"] = userTickets.Where(x => x.Priority == "High").ToList().Count();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}