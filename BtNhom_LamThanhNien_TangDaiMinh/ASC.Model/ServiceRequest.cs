using System.ComponentModel.DataAnnotations;

namespace ASC.Model
{
    public class ServiceRequest : BaseEntity, IAuditTracker
    {
        [Required]
        public string RequestName { get; set; }

        public string Description { get; set; }

        [Required]
        public string Status { get; set; }  // "New", "InProgress", "Completed"

        public string AssignedTo { get; set; } // Id của kỹ sư

        // IAuditTracker
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
