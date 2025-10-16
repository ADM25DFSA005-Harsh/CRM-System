namespace MarketingAutomation.Models
{
    public class Campaign
    {
        public int CampaignID { get; set; }
        public string CampaignName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public decimal Budget { get; set; }
        public string ScheduleType { get; set; }
        public int? Impressions { get; set; } // nullable
        public ICollection<CustomerResponse>? CustomerResponse { get; set; }

    }

}
