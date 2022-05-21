using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using BugTracker.Models;

namespace BugTracker.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }

        public string GroupName { get; set; }

        public ApplicationUser()
        {
            this.Projects = new HashSet<Project>();
            this.Tickets = new HashSet<Ticket>();
        }

        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

        public string FullName
        {
            get { return this.FirstName + " " + this.LastName; }
        }
    }
}
