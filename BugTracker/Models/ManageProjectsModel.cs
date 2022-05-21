using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models
{
    public class ManageProjectsModel
    {
        public SelectList selectListItems { get; set; }

        public List<SelectListItem> selectListItemsList { get; set; }
    }
}
