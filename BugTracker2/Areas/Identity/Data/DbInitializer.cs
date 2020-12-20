using BugTracker2.Models;
using System;
using System.Linq;

namespace BugTracker2.Areas.Identity.Data
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
            new Status() { StatusName = "New", Step = 1 },
            new Status() { StatusName = "Assigned", Step = 2 },
            new Status() { StatusName = "Open", Step = 3 },
            new Status() { StatusName = "Fixed", Step = 4 },
            new Status() { StatusName = "Pending Retest", Step = 5 },
            new Status() { StatusName = "Retest", Step = 6 },
            new Status() { StatusName = "Reopened", Step = 7 },
            new Status() { StatusName = "Verified", Step = 8 },
            new Status() { StatusName = "Closed", Step = 9 }
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
            new Privilege() { PrivilegeLevel = 1, PrivilegeName = "Read Only" },
            new Privilege() { PrivilegeLevel = 2, PrivilegeName = "Create, Read, Update, and Delete" },
            new Privilege() { PrivilegeLevel = 3, PrivilegeName = "CRUD and User Assign" },
            new Privilege() { PrivilegeLevel = 4, PrivilegeName = "Master" }
            };

            context.Privilege.AddRange(privileges);

            var user = new User()
            {
                FirstName = "--UnAssigned--",
                LastName = "",
                Email = ""
            };

            context.Users.Add(user);

            context.SaveChanges();
        }
    }
}
