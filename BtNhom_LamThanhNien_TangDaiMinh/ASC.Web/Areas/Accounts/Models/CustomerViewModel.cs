namespace ASC.Web.Areas.Accounts.Models;

public class CustomerViewModel
{
    public List<CustomerRegistrationViewModel> Customers { get; set; } = [];
    public CustomerRegistrationViewModel Registration { get; set; } = new();
}
