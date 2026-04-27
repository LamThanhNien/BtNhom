using System.ComponentModel.DataAnnotations;

namespace ASC.Model
{
    public class MasterDataValue : BaseEntity, IAuditTracker
    {
        [Required]
        public string Value { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public string MasterDataKeyId { get; set; } = string.Empty;

        public virtual MasterDataKey MasterDataKey { get; set; } = null!;

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public string UpdatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedDate { get; set; }
    }
}
