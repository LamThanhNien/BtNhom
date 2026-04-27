using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Model
{
    public class ServiceRequest : BaseEntity, IAuditTracker
    {
        public ServiceRequest()
        {
        }

        public ServiceRequest(string email)
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = email;
        }

        [NotMapped]
        public string RowKey
        {
            get => Id;
            set => Id = value;
        }

        [Required]
        public string PartitionKey { get; set; } = string.Empty;

        [Required]
        public string VehicleName { get; set; } = string.Empty;

        [Required]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;

        [Required]
        public string RequestedServices { get; set; } = string.Empty;

        public DateTime? RequestedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string? ServiceEngineer { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public string UpdatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedDate { get; set; }
    }
}
