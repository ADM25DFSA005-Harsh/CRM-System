using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CRMProject.Models
{
    public class SupportTicket
    {

        [Key]
        public int TicketID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public string? Issue { get; set; }

        [Required]
        [MaxLength(500)]
        public string? IssueDescription { get; set; }

        [MaxLength(100)]
        public string? AssignedAgent { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Status { get; set; }



    }
}

