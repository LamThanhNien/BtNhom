namespace ASC.Business.Models
{
    public class MasterDataValueModel
    {
        public string Id { get; set; } = string.Empty;
        public string MasterDataKeyId { get; set; } = string.Empty;
        public string MasterDataKey { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}

