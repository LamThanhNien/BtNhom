using System.ComponentModel.DataAnnotations;

namespace ASC.Model
{
    public class MasterDataKey : BaseEntity, IAuditTracker
    {
        [Required]
        public string Key { get; set; }        // Ví dụ: "ServiceStatus"
        public string Description { get; set; }

        // Navigation property
        public virtual ICollection<MasterDataValue> MasterDataValues { get; set; }

        // IAuditTracker
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}