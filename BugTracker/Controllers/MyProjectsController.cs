using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.RolesClasses;

namespace BugTracker.Controllers
{
    [Authorize]
    public class MyProjectsController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public IHttpContextAccessor _httpContextAccessor;
        public MyProjectsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(string? searchString, string? currentFilter, int? pageNumber)
        {
            MyProjectsModel model = new MyProjectsModel();
            int pageSize = 1;
            if (searchString != null)
            {
                pageNumber = 1;
            } else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            IQueryable<Project> projects;
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId);
            projects = _db.Projects.Where(x => x.ApplicationUsers.Contains(user));

            if (!String.IsNullOrEmpty(searchString))
            {
                
                projects = projects.Where(x => x.Title.ToLower().Contains(searchString.ToLower()));
                model.paginatedProjects = await PaginatedList<Project>.CreateAsync(projects, pageNumber ?? 1, pageSize);
            } else
            {
                model.paginatedProjects = await PaginatedList<Project>.CreateAsync(projects, pageNumber ?? 1, pageSize); 
            }
            return View(model);
        }  

        public async Task<IActionResult> DeleteProject(string projectID)
        {
            Project project = await _db.Projects.FindAsync(projectID);
            _db.Projects.Remove(project);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
