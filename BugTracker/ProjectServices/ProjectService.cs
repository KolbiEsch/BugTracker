using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.ProjectServices
{
    public class ProjectService
    {
        private readonly ApplicationDbContext _db;

        public ProjectService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Project getProjectById(string projectId)
        {
            foreach(var project in _db.Projects)
            {
                if (project.ProjectID == projectId)
                {
                    return project;
                }
            }
            return null;
        }

    }
}
