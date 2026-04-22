namespace ASC.Web.Areas.Configuration.Models
{
    public class MasterValuesViewModel
    {
        public List<MasterDataValueViewModel> MasterValues { get; set; } = new();
        public MasterDataValueViewModel MasterValueInContext { get; set; } = new();
        public bool IsEdit { get; set; }
    }
}
