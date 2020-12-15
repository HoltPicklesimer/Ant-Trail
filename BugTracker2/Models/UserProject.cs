using System.ComponentModel.DataAnnotations.Schema;
using BugTracker2.Areas.Identity.Data;

namespace BugTracker2.Models
{
    public class UserProject
    {
        public int UserProjectId { get; set; }
        public string UserId { get; set; }
        public int ProjectId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Project Project { get; set; }
    }
}
