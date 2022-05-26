using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugTracker.Data;
using BugTracker.RolesClasses;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class ManageUserRolesModel
    {
        public List<IdentityRole> Roles { get; set; }

        public SelectList selectListItems { get; set; }

        public PaginatedList<ApplicationUser> paginatedAppUsers { get; set; }

        [Display(Name = "Role")]
        public string? RoleName { get; set; }
    }
}
