using ASC.Web.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Areas.ServiceRequests.Controllers;

[Area("ServiceRequests")]
public class DashboardController : BaseController
{
    public IActionResult Dashboard()
    {
        return View();
    }
}
