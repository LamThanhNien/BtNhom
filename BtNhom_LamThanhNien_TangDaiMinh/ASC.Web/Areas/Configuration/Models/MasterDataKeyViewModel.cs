using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.Configuration.Models
{
    public class MasterDataKeyViewModel
    {
        public string? RowKey { get; set; }
        public string? PartitionKey { get; set; }
        public bool IsActive { get; set; } = true;

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
