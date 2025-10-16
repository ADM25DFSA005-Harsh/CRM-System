using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CRM.Models
{
    public class CustomerProfile
    {
        [Key]
        [Display(Name = "ID")]
        public int CustomerID { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        [Display(Name = "Join Date")]
        public string? JoinDate { get; set; }

        [Display(Name = "Date of Birth")]
        public string? DateOfBirth { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        [Display(Name = "Phone No")]
        public string? PhoneNumber { get; set; }
        public string? Region { get; set; }

        [Display(Name = "Purchase History")]
        public string? PurchaseHistory { get; set; }
    }
}
