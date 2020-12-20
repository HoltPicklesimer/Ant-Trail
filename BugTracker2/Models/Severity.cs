namespace BugTracker2.Models
{
    public class Severity
    {
        public int SeverityId { get; set; }
        public string SeverityName { get; set; }
        public int Priority { get; set; }
        public string SeverityDisplay {
            get
            {
                return Priority + " - " + SeverityName;
            }
        }
    }
}
