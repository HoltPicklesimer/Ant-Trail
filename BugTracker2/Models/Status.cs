namespace BugTracker2.Models
{
    public class Status
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public int Step { get; set; }
        public string StatusDisplay
        {
            get { return Step + " - " + StatusName; }
        }
    }
}
