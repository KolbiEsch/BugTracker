using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    public class DemoUserController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;

        public DemoUserController(
            RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager, 
            ApplicationDbContext db, 
            SignInManager<ApplicationUser> signInManager, 
            IUserStore<ApplicationUser> userStore)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
            _signInManager = signInManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAdminDemo()
        {
            await SignInUser("Admin");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        
        public async Task<IActionResult> LoginSubmitterDemo()
        {
            await SignInUser("Submitter");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> LoginDeveloperDemo()
        {
            await SignInUser("Developer");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SwitchUser(string groupName, string userToSwitchTo)
        {
            await _signInManager.SignOutAsync();
            foreach(var user in _userManager.Users.ToList())
            {
                if (user.GroupName == groupName && user.FirstName == userToSwitchTo)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> EndDemo(string groupName)
        {
            await _signInManager.SignOutAsync();
            foreach(var user in _userManager.Users.ToList())
            {
                if (user.GroupName == groupName)
                {
                    await DeleteTicketsOfUser(user);
                    _db.ApplicationUsers.Remove(user);
                }
            }

            foreach(var project in _db.Projects)
            {
                if (project.GroupName == groupName)
                {
                    _db.Projects.Remove(project);
                }
            }
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        private async Task DeleteTicketsOfUser(ApplicationUser user)
        {
            foreach(var ticket in user.Tickets)
            {
               await DeleteFilesFromTicket(ticket);
                _db.Remove(ticket);
            }
            _db.SaveChanges();
        }

        private async Task DeleteFilesFromTicket(Ticket ticket)
        {
            foreach (var filePath in ticket.FilePaths)
            {
                FileInfo file = new FileInfo(filePath.HiddenPath);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            _db.SaveChanges();
        }

        private async Task<List<ApplicationUser>> CreateDemoEnvironment()
        {
            var groupName = "Demo." + Guid.NewGuid().ToString("N");
            var AdminEmail = Guid.NewGuid().ToString("N");
            var SubmitterEmail = Guid.NewGuid().ToString("N");
            var DeveloperEmail = Guid.NewGuid().ToString("N");
            

            List<ApplicationUser> DemoUsers = new();

            var AdminUser = CreateDemoUser("Admin", AdminEmail, groupName).Result;
            var SubmitterUser = CreateDemoUser("Submitter", SubmitterEmail, groupName).Result;
            var DeveloperUser = CreateDemoUser("Developer", DeveloperEmail, groupName).Result;

            DemoUsers.Add(AdminUser);
            DemoUsers.Add(SubmitterUser);
            DemoUsers.Add(DeveloperUser);

            var project = CreateDemoProject("Test Project", "To Test", "Test", groupName);
            AddUsersToProject(project, DemoUsers);
            var ticketForAdmin = CreateDemoTicket(project.ProjectID, AdminUser.Id, "Bug", "Admin Ticket", "This is a ticket", "Low", SubmitterUser.Id);
            var ticketForDeveloper = CreateDemoTicket(project.ProjectID, DeveloperUser.Id, "Feature", "Developer Ticket", "This is a ticket", "Medium", SubmitterUser.Id);
            _db.SaveChanges();

            return DemoUsers;
        }

        private Project CreateDemoProject(string title, string centralFocus, string description, string groupName)
        {
            var project = new Project
            {
                Title = title,
                CentralFocus = centralFocus,
                Description = description,
                GroupName = groupName,
                CreatedDate = DateTime.Now,
                ProjectID = Guid.NewGuid().ToString("N")
            };

            _db.Projects.Add(project);
            _db.SaveChanges();
            return project;
        }

        private async Task<Ticket> CreateDemoTicket(string projectId, string userId, string Type, string Title, string Description, string Priority, string assignedByID)
        {
            Ticket ticket = new Ticket
            {
                Type = Type,
                Title = Title,
                Description = Description,
                Priority = Priority,
                TicketID = Guid.NewGuid().ToString("N"),
                DateCreated = DateTime.Now,
                AssignedByID = assignedByID,
                ApplicationUserId = userId,
                ProjectID = projectId,
            };

            var user = await _userManager.FindByIdAsync(userId);
            user.Tickets.Add(ticket);
            _db.Tickets.Add(ticket);
            _db.SaveChanges();
            return ticket;
        }

        private void AddUsersToProject(Project project, List<ApplicationUser> users)
        {
            foreach(var user in users)
            {
                user.Projects.Add(project);
                project.ApplicationUsers.Add(user);
            }
        }

        private async Task SignInUser(string role)
        {
            var users = CreateDemoEnvironment().Result;
            foreach (var user in users)
            {
                if (user.FirstName == role)
                {
                    await _signInManager.SignInAsync(user, true);
                }
            }
        }

        private async Task<ApplicationUser> CreateDemoUser(string name, string email, string groupName)
        {
            var DemoUser = new ApplicationUser
            {
                FirstName = name,
                LastName = "Demo",
                Email = email,
                GroupName = groupName
            };

            await _userStore.SetUserNameAsync(DemoUser, email, CancellationToken.None);
            await _emailStore.SetEmailAsync(DemoUser, email, CancellationToken.None);
            await _userManager.CreateAsync(DemoUser);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(DemoUser);
            await _userManager.ConfirmEmailAsync(DemoUser, code);

            await _userManager.AddToRoleAsync(DemoUser, name);

            return DemoUser;

        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}