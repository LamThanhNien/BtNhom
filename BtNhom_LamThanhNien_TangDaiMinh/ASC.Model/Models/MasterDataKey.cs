using System.ComponentModel.DataAnnotations;

namespace ASC.Model
{
    public class MasterDataKey : BaseEntity, IAuditTracker
    {
        [Required]
        public string Key { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<MasterDataValue> MasterDataValues { get; set; } = new List<MasterDataValue>();

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public string UpdatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedDate { get; set; }
    }
}
