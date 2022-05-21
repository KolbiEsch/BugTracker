using System.ComponentModel.DataAnnotations.Schema;
using BugTracker.Data;

namespace BugTracker.Models
{
    public class Project
    {
        public Project()
        {
            this.ApplicationUsers = new HashSet<ApplicationUser>();
            this.Tickets = new HashSet<Ticket>();
        }
        public string? Title { get; set; }

        public string ProjectID { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CentralFocus { get; set; }

        public string Description { get; set; }

        public string GroupName { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

    }
}
