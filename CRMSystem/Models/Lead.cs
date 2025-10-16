using System.ComponentModel.DataAnnotations;

namespace SalesAutomation.Models
{
    public class Lead
    {
        [Key]
        public int LeadID { get; set; }
        public string? Salutation { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? JobTitle { get; set; }
        //public string? Organization { get; set; } // Optional if client is individual

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? MobileNumber { get; set; }

        public string? Region { get; set; }
        public string? Source { get; set; }

        public string? Status { get; set; } // New, Contacted, Qualified, Converted
        public string? AssignedTo { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Activity logs (nullable)
        public string? EmailHistory { get; set; } // e.g., "10-Oct: Welcome|11-Oct: Follow-up"
        public string? CallHistory { get; set; }  // e.g., "10-Oct: Called client|12-Oct: Follow-up call"
        public string? Notes { get; set; }        // e.g., "10-Oct: Discussed pricing|11-Oct: Sent proposal"
        public string? Tasks { get; set; }        // e.g., "10-Oct: Prepare quote [High]|12-Oct: Schedule meeting [Low]"
    }
}


