using ASC.Web.Configuration;
using ASC.Web.Controllers;
using ASC.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ASC.Tests;

public class HomeControllerTests
{
    [Fact]
    public void Index_ReturnsViewResult_AndSetsApplicationTitle()
    {
        var controller = CreateController();

        var result = controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        Assert.Null(view.Model);
        Assert.Equal("Automobile Service Center", controller.ViewBag.Title);
    }

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        var controller = CreateController();

        var result = controller.Privacy();

        var view = Assert.IsType<ViewResult>(result);
        Assert.Null(view.Model);
    }

    [Fact]
    public void Error_ReturnsViewResult_WithErrorModel()
    {
        var controller = CreateController();

        var result = controller.Error();

        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<ErrorViewModel>(view.Model);
    }

    private static HomeController CreateController()
    {
        var settings = Options.Create(new ApplicationSettings
        {
            ApplicationTitle = "Automobile Service Center"
        });

        var controller = new HomeController(NullLogger<HomeController>.Instance, settings)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        return controller;
    }
}
