namespace ASC.Web.Areas.Accounts.Models;

public class ServiceEngineerViewModel
{
    public List<ServiceEngineerRegistrationViewModel> ServiceEngineers { get; set; } = [];
    public ServiceEngineerRegistrationViewModel Registration { get; set; } = new();
}
