using Microsoft.AspNetCore.Identity;
using BugTracker.Data;

namespace BugTracker.Models
{
    public class UserRole
    {
        UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public UserRole(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<string> GetUserRole(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault("").ToString();
      
        }
    }
}
