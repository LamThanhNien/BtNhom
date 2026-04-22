namespace ASC.Web.Areas.Configuration.Models
{
    public class MasterKeysViewModel
    {
        public List<MasterDataKeyViewModel> MasterKeys { get; set; } = new();
        public MasterDataKeyViewModel MasterKeyInContext { get; set; } = new();
        public bool IsEdit { get; set; }
    }
}
