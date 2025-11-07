namespace CRMSystem.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Report
    {
        [Key]
        public int ReportID { get; set; }

        [Required]
        [StringLength(50)]
        public string? ReportType { get; set; } // Sales, Support, Marketing

        [Required]
        public DateTime GeneratedDate { get; set; }

        public string? DataPoints { get; set; } // Store as JSON or plain text
    }
}
