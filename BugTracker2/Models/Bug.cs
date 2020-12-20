using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BugTracker2.Areas.Identity.Data;

namespace BugTracker2.Models
{
    public class Bug
    {
        public int BugId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Bug Name")]
        public string BugName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ReportDate { get; set; }

        [Required]
        [Display(Name = "Severity")]
        public int SeverityId { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Behavior")]
        public string Behavior { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Steps to Reproduce")]
        public string StepsDescription { get; set; }

        [Display(Name = "User Reported")]
        public string UserReportedId { get; set; }

        [Display(Name = "User Assigned")]
        public string UserAssignedId { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; }

        [Display(Name = "Project")]
        public int ProjectId { get; set; }


        // Navigation Properties
        public Severity Severity { get; set; }

        [ForeignKey("UserReportedId")]
        [InverseProperty("BugsReported")]
        public User UserReported { get; set; }

        [ForeignKey("UserAssignedId")]
        [InverseProperty("BugsAssigned")]
        public User UserAssigned { get; set; }
        public Status Status { get; set; }

        public Project Project { get; set; }
    }
}
