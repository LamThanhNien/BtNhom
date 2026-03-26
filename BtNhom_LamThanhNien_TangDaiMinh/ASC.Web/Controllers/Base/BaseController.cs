using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Controllers.Base;

[Authorize]
public class BaseController : Controller
{
}
