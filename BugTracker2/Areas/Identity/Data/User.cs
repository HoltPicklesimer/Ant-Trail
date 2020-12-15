using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using BugTracker2.Models;

namespace BugTracker2.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        // Navigation Properties
        // Navigation Properties
        public ICollection<UserGroup> UserGroups { get; set; }
        public ICollection<UserProject> UserProjects { get; set; }
        public ICollection<Bug> BugsReported { get; set; }
        public ICollection<Bug> BugsAssigned { get; set; }
    }
}
