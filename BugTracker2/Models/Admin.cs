using System.ComponentModel.DataAnnotations;
using BugTracker2.Areas.Identity.Data;

namespace BugTracker2.Models
{
    public class Admin
    {
        public int AdminId { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; }

        [Display(Name = "Group")]
        public int GroupId { get; set; }

        [Display(Name = "Privilege Level")]
        public int PrivilegeId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Group Group { get; set; }
    }
}
