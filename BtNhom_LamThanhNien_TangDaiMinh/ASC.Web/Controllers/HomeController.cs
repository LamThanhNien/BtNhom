using ASC.Utilities;
using ASC.Web.Controllers.Base;
using ASC.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ASC.Web.Controllers;

public class HomeController : AnonymousController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IOptions<ApplicationSettings> _settings;

    public HomeController(ILogger<HomeController> logger, IOptions<ApplicationSettings> settings)
    {
        _logger = logger;
        _settings = settings;
    }

    public IActionResult Index()
    {
        HttpContext.Session.SetObject("AppSettings", _settings.Value);
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
