using ASC.Model;

namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class DashboardViewModel
    {
        public List<ServiceRequest> ServiceRequests { get; set; } = new();
    }
}
