namespace ASC.Business.Models
{
    public class MasterDataCache
    {
        public List<MasterDataKeyModel> Keys { get; set; } = new();
        public List<MasterDataValueModel> Values { get; set; } = new();
    }
}
