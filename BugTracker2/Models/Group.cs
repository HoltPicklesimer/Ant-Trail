using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTracker2.Models
{
   public class Group
   {
      public int GroupId { get; set; }

      [Required]
      [StringLength(100)]
      [Display(Name = "Group Name")]
      public string GroupName { get; set; }

      [Required]
      [StringLength(500)]
      [Display(Name = "Description")]
      public string GroupDescription { get; set; }

      // Navigation Properties
      public ICollection<GroupProject> GroupProjects { get; set; }
      public ICollection<UserGroup> UserGroups { get; set; }
   }
}
