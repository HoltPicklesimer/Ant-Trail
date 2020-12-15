using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTracker2.Models
{
   public class Project
   {
      public int ProjectId { get; set; }

      [Required]
      [StringLength(100)]
      [Display(Name = "Project Name")]
      public string ProjectName { get; set; }

      [Required]
      [StringLength(500)]
      [Display(Name = "Description")]
      public string ProjectDescription { get; set; }

      // Navigation Properties
      public ICollection<UserProject> UserProjects { get; set; }
      public ICollection<GroupProject> GroupProjects { get; set; }
   }
}
