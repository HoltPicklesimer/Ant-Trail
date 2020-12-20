using System.ComponentModel.DataAnnotations;

namespace BugTracker2.Models
{
    public class Privilege
    {
        public int PrivilegeId { get; set; }
        public string PrivilegeName { get; set; }
        public int PrivilegeLevel { get; set; }
        public string PrivilegeDisplay
        {
            get { return PrivilegeLevel + " - " + PrivilegeName; }
        }
    }
}
