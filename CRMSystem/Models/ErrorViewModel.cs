namespace CRMSystem.Models
{
    public class ErrorViewModel
    {
        public string? RequestReportID { get; set; }

        public bool ShowRequestReportID => !string.IsNullOrEmpty(RequestReportID);
    }
}
