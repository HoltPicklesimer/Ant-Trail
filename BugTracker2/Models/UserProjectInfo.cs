using System.ComponentModel.DataAnnotations;
using BugTracker2.Areas.Identity.Data;

namespace BugTracker2.Models
{
    public class UserProjectInfo
    {
        public int UserProjectInfoId { get; set; }

        [Display(Name = "Enter User Email")]
        public string UserId { get; set; }
        public int ProjectId { get; set; }

        [Display(Name = "Privilege Level")]
        public int PrivilegeId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Project Project { get; set; }
        public Privilege Privilege { get; set; }
    }
}
