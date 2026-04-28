using ASC.Utilities;
using ASC.Web.Configuration;
using ASC.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ASC.Web.Controllers;

public class HomeController : AnonymousController
{
    private IOptions<ApplicationSettings> _settings;

    public HomeController(IOptions<ApplicationSettings> settings)
    {
        _settings = settings;
    }

    public IActionResult Index()
    {
        //// Set Session
        HttpContext.Session.SetSession("Test", _settings.Value);
        //// Get Session
        var settings = HttpContext.Session.GetSession<ApplicationSettings>("Test");
        //// Usage of IOptions
        ViewBag.Title = _settings.Value.ApplicationTitle;
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
