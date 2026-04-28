using ASC.Model;

namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class DashboardViewModel
    {
        public List<ServiceRequest> ServiceRequests { get; set; } = new();
        public List<string> ServiceEngineerEmails { get; set; } = new();
    }
}
