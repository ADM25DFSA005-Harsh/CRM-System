using System.ComponentModel.DataAnnotations;
using CRM.Models;
namespace SalesAutomation.Models
{
    public class SalesOpportunity
    {
        [Key]
        public int OpportunityID { get; set; }
        //public int? CustomerID { get; set; } // Nullable foreign key
        public string? SalesStage { get; set; }
        public decimal? EstimatedValue { get; set; }
        public DateTime? ClosingDate { get; set; }

        //public CustomerProfile? CustomerProfile { get; set; }
        //public string? Status { get; set; }
        public string? AssignedRep { get; set; }
        public int? LeadId { get; set; } // 🔗 Foreign key to Lead
        public Lead? Lead { get; set; }  // 🔗 Navigation property
    }
}
