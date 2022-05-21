using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugTracker.Data;
using BugTracker.RolesClasses;

namespace BugTracker.Models
{
    public class ViewModel
    {
        public  CreateRoleViewModel  CreateRoleViewModel { get; set; }

        public List<IdentityRole> Roles { get; set; }

        public SelectList selectListItems { get; set; }

        public UserRole userRole { get; set; }

        public PaginatedList<ApplicationUser> paginatedAppUsers { get; set; }
    }
}
