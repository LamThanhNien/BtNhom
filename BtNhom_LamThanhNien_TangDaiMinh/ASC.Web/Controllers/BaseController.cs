using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Controllers;

[Authorize]
public abstract class BaseController : Controller
{
}
