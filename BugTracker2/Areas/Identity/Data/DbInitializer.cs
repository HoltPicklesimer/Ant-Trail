using BugTracker2.Models;
using System;
using System.Linq;

namespace BugTracker2.Data
{
   public class DbInitializer
   {
      public static void Initialize(BugTracker2Context context)
      {
         context.Database.EnsureCreated();

         if (context.Status.Any())
         {
            return;   // DB has been seeded
         }

         var statuses = new Status[]
         {
            new Status() { StatusName = "New" },
            new Status() { StatusName = "Assigned" },
            new Status() { StatusName = "Open" },
            new Status() { StatusName = "Fixed" },
            new Status() { StatusName = "Pending Retest" },
            new Status() { StatusName = "Retest" },
            new Status() { StatusName = "Reopened" },
            new Status() { StatusName = "Verified" },
            new Status() { StatusName = "Closed" }
         };

         context.Status.AddRange(statuses);

         var severities = new Severity[]
         {
            new Severity() { Priority = 1, SeverityName = "Critical" },
            new Severity() { Priority = 2, SeverityName = "High" },
            new Severity() { Priority = 3, SeverityName = "Medium" },
            new Severity() { Priority = 4, SeverityName = "Low" }
         };

         context.Severity.AddRange(severities);

         var privileges = new Privilege[]
         {
            new Privilege() { PrivilegeLevel = 0, PrivilegeName = "No Access" },
            new Privilege() { PrivilegeLevel = 1, PrivilegeName = "Read Only" },
            new Privilege() { PrivilegeLevel = 2, PrivilegeName = "Read and Update" },
            new Privilege() { PrivilegeLevel = 3, PrivilegeName = "Create, Read, Update" },
            new Privilege() { PrivilegeLevel = 4, PrivilegeName = "Create, Read, Update, and Delete" },
            new Privilege() { PrivilegeLevel = 5, PrivilegeName = "Administrator" },
            new Privilege() { PrivilegeLevel = 6, PrivilegeName = "Master" }
         };

         context.Privilege.AddRange(privileges);

         context.SaveChanges();
      }
   }
}
