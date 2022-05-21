using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.RolesClasses;


namespace BugTracker.Controllers
{
    [Authorize]
    public class MyTicketsController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _db;
        public MyTicketsController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> Index(string? searchString, string? currentFilter, int? pageNumber)
        {
            int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
            } else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            IQueryable<Ticket> tickets;
            tickets = _db.Tickets.Where(x => x.ApplicationUserId == _userManager.GetUserId(HttpContext.User));
            
            

            MyTicketsModel model = new MyTicketsModel(_db);
            if (!String.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(x => x.Title.ToLower().Contains(searchString.ToLower()));
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            model.Tickets = await PaginatedList<Ticket>.CreateAsync(tickets, pageNumber ?? 1, pageSize);
            model.ProjectModel = new List<Project>();
            model.ProjectModel.Add(new Project { ProjectID = "0", Title = "Select Project", CentralFocus = "Default", Description = "Default", CreatedDate = DateTime.Now });
            foreach(var project in _db.Projects)
            {
                if (user.GroupName == project.GroupName)
                {
                    model.ProjectModel.Add(project);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewTicket(string projectId, string userId, string Type, string Title, string Description, string Priority)
        {
            
            var Ticket = new Ticket
            {
                Type = Type,
                Title = Title,
                Description = Description,
                Priority = Priority,
                TicketID = Guid.NewGuid().ToString("N"),
                DateCreated = DateTime.Now,
                AssignedByID = _userManager.GetUserId(HttpContext.User),
                ApplicationUserId = userId,
                ProjectID = projectId,
            };
            var user = await _userManager.FindByIdAsync(userId);
            user.Tickets.Add(Ticket);
            _db.Tickets.Add(Ticket);
            _db.SaveChanges();
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> GetApplicationUserByProjectId(string projectId)
        {
            var project = await _db.Projects.FindAsync(projectId);
            List<ApplicationUser> objApplicationUser = new List<ApplicationUser>();
            objApplicationUser = GetAllUsers().Where(x => x.Projects.Contains(project)).ToList();
            SelectList obgApplicationUser = new SelectList(objApplicationUser, "Id", "FullName");
            return Json(obgApplicationUser);
        }

        public List<ApplicationUser> GetAllUsers()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            foreach(var user in _userManager.Users)
            {
                users.Add(user);
            }
            return users;
        }

       
    }
}
