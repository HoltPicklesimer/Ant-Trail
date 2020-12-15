using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker2.Models
{
   public class GroupProject
   {
      public int GroupProjectId { get; set; }
      public int GroupId { get; set; }
      public int ProjectId { get; set; }

      // Navigation Properties
      public Group Group { get; set; }
      public Project Project { get; set; }
   }
}
