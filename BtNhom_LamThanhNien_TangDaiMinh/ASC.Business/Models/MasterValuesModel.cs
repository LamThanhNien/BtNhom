namespace ASC.Business.Models
{
    public class MasterValuesModel
    {
        public string MasterDataKeyId { get; set; } = string.Empty;
        public string MasterDataKey { get; set; } = string.Empty;
        public List<MasterDataValueModel> MasterDataValues { get; set; } = new();
    }
}

