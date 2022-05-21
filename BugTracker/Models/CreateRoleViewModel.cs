using System.ComponentModel.DataAnnotations;

namespace BugTracker.Data
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string? RoleName { get; set; }
    }
}
