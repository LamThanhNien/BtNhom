using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class NewServiceRequestViewModel
    {
        [Required]
        [Display(Name = "Customer Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vehicle Name")]
        public string VehicleName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Requested Services")]
        public string RequestedServices { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Requested Date")]
        public DateTime? RequestedDate { get; set; }
    }
}
