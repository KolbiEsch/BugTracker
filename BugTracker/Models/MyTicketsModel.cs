using Microsoft.AspNetCore.Mvc.Rendering;
using BugTracker.RolesClasses;
using BugTracker.Data;

namespace BugTracker.Models
{
    public class MyTicketsModel
    {
        private readonly ApplicationDbContext _context;
        public MyTicketsModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public List<Project> ProjectModel { get; set; }

        public PaginatedList<Ticket> Tickets { get; set; }

        public Ticket ticket { get; set; }

        public SelectList FilteredApplicationUser { get; set; }

        public string GetProjectTitleById(string projectId)
        {
            string title = _context.Projects.Find(projectId).Title;   
            return title;
        }

        public string GetUserFullNameById(string userId)
        {
            return _context.ApplicationUsers.Find(userId).FullName;
        }
    }
}
