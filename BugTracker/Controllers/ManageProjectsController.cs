using Microsoft.AspNetCore.Mvc;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageProjectsController : Controller
    {
        public ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        

        public ManageProjectsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            ManageProjectsModel model = new ManageProjectsModel();
            List<Project> projects = new List<Project>();
            var user = await _userManager.GetUserAsync(HttpContext.User);

            foreach (var project in _db.Projects)
            {
                if (project.GroupName == user.GroupName)
                {
                    projects.Add(project);
                }
                
            }
            
            model.selectListItems = new SelectList(projects, "Title", "Title");
            model.selectListItemsList = new List<SelectListItem>();
            foreach(var appuser in _db.Users)
            {
                if (appuser.GroupName == user.GroupName)
                {
                    model.selectListItemsList.Add(new SelectListItem
                    {
                        Text = appuser.FullName,
                        Value = appuser.Id
                    });
                }   
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewProject(string Title, string CentralFocus, string Description)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var project = new Project
            {
                Title = Title,
                CentralFocus = CentralFocus,
                Description = Description,
                CreatedDate = DateTime.Now,
                ProjectID = Guid.NewGuid().ToString("N"),
                GroupName = user.GroupName 
            };
            
            _db.Projects.Add(project);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToProject(string Title, string[] users)
        {
            Project project = GetProjectByTitle(Title);
            List<SelectListItem> usersList = new List<SelectListItem>();
            foreach (var user in _db.Users)
            {
                usersList.Add(new SelectListItem
                {
                    Text = user.FullName,
                    Value = user.Id
                });
            }
            
            foreach(SelectListItem user in usersList)
            {
                if (users.Contains(user.Value))
                {
                    var fullUser = await _userManager.FindByIdAsync(user.Value);
                    project.ApplicationUsers.Add(fullUser);
                    fullUser.Projects.Add(project);
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
            
        }

        private Project GetProjectByTitle(string Title)
        {
            foreach(var project in _db.Projects)
            {
                if (project.Title == Title)
                {
                    return project;
                }
            }
            return null;
        }

    }
}
