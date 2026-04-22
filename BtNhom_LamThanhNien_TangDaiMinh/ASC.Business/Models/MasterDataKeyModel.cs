namespace ASC.Business.Models
{
    public class MasterDataKeyModel
    {
        public string Id { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TotalValues { get; set; }
    }
}

