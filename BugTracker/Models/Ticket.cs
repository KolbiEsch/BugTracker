using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using BugTracker.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Models
{
    public class Ticket
    {
        public Ticket()
        {
            this.FilePaths = new HashSet<FilePath>();
        }
        
        public string TicketID { get; set; }

        public string Type { get; set; }

        public string Priority { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public string AssignedByID { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual Project Project { get; set; }
        
        public virtual ICollection<FilePath> FilePaths { get; set; }

        public string ApplicationUserId { get; set; }

        public string ProjectID { get; set; }

    }
}


