using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CRMSystem.Data;
using CRM.Models;

namespace MarketingAutomation.Models
{
    public class CustomerResponse
    {
        [Key]
        public int ResponseID { get; set; }

        // Foreign key to CustomerProfile
        [Required]
        public int CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public CustomerProfile Customer { get; set; }

        // Foreign key to Campaign
        [Required]
        public int CampaignID { get; set; }

        [ForeignKey("CampaignID")]
        public Campaign Campaign { get; set; }

        // Type of response: e.g., Clicked, Converted, Unsubscribed
        [Required]
        public string ResponseType { get; set; }

       

        [Range(1, 5)]
        public int? Rating { get; set; } // Optional rating


        
    }
}
