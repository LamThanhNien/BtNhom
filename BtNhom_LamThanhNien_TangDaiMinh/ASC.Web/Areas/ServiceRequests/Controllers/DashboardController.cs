using ASC.Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Areas.ServiceRequests.Controllers;

[Area("ServiceRequests")]
[Authorize(Roles = "Admin,Engineer,User")]
public class DashboardController : BaseController
{
    public IActionResult Dashboard()
    {
        return View();
    }
}
