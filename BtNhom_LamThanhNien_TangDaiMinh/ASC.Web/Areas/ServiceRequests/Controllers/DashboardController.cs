using ASC.Web.Controllers;
using ASC.Web.Extensions;
using ASC.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Areas.ServiceRequests.Controllers;

[Area("ServiceRequests")]
[Authorize(Roles = "Admin,Engineer,User")]
public class DashboardController : BaseController
{
    public IActionResult Dashboard()
    {
        HttpContext.Session.SetObject(CurrentUser.SessionKey, User.ToCurrentUser());
        return View();
    }
}
