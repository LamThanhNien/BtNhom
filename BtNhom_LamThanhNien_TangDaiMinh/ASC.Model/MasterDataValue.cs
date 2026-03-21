using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Model
{
    public class MasterDataValue : BaseEntity, IAuditTracker
    {
        [Required]
        public string Value { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        // Foreign key
        public string MasterDataKeyId { get; set; }
        public virtual MasterDataKey MasterDataKey { get; set; }

        // IAuditTracker
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}