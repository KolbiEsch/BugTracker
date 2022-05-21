using BugTracker.RolesClasses;

namespace BugTracker.Models
{
    public class MyProjectsModel
    {
        public PaginatedList<Project> paginatedProjects { get; set; }
    }
}
