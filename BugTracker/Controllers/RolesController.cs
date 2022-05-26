using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BugTracker.Data;
using System.Threading.Tasks;
using BugTracker.Models;
using System.Dynamic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugTracker.RolesClasses;
using Microsoft.AspNetCore.Authorization;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ManageUserRolesModel myModel = new ManageUserRolesModel();
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public RolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db) {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> RoleManagement(string? searchString, string? currentFilter, int? pageNumber)
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

            ManageUserRolesModel model = new ManageUserRolesModel();
            
            var userId = _userManager.GetUserId(HttpContext.User);
            var contextUser = await _userManager.FindByIdAsync(userId);
           
            IQueryable<ApplicationUser> usersList = _userManager.Users.Where(x => x.GroupName == contextUser.GroupName);
            if (!String.IsNullOrEmpty(searchString))
            {
                IQueryable<ApplicationUser> list = usersList.Where(u => u.FirstName.ToLower().Contains(searchString.ToLower())
                || u.LastName.ToLower().Contains(searchString.ToLower()));
                model.paginatedAppUsers = await PaginatedList<ApplicationUser>.CreateAsync(list, pageNumber ?? 1, pageSize);
            } else
            {
                model.paginatedAppUsers = await PaginatedList<ApplicationUser>.CreateAsync(usersList, pageNumber ?? 1, pageSize);
            }

            //Instantiate Models
            model.selectListItems = new SelectList(usersList.ToList(), "Id", "FullName");
            model.Roles = _roleManager.Roles.ToList();
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(ManageUserRolesModel model)
        {
            var role = new IdentityRole(model.RoleName);

            await _roleManager.CreateAsync(role);

            return RedirectToAction("RoleManagement");
        }
        
        [HttpPost]
        public async Task<IActionResult> AssignUserRole(string Id, string Name)  
        {
            var user = await _userManager.FindByIdAsync(Id);
            await _userManager.AddToRoleAsync(user, Name);
            return RedirectToAction("RoleManagement");
        }
        
    }
}
