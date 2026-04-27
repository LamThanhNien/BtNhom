using System.ComponentModel.DataAnnotations;

namespace ASC.Model
{
    public class Books : BaseEntity, IAuditTracker
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
    }
}
