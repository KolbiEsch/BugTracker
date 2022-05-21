using Microsoft.EntityFrameworkCore;

namespace BugTracker.Models
{
    
    public class FilePath
    {
        public string ID { get; set; }
        public string HiddenPath { get; set; }

        public string UnhiddenPath { get; set; }

        public virtual Ticket ticket { get; set; }

    }
}
