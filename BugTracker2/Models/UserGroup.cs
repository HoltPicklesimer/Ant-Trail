using System.ComponentModel.DataAnnotations.Schema;
using BugTracker2.Areas.Identity.Data;

namespace BugTracker2.Models
{
   public class UserGroup
   {
      public int UserGroupId { get; set; }
      public string UserId { get; set; }
      public int GroupId { get; set; }

      // Navigation Properties
      public User User { get; set; }
      public Group Group { get; set; }
   }
}
