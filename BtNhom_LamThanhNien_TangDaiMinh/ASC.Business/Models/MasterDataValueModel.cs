namespace ASC.Business.Models
{
    public class MasterDataValueModel
    {
        public string RowKey { get; set; } = string.Empty;
        public string PartitionKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
