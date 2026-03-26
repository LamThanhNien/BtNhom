using ASC.Tests.Fakes;
using ASC.Web.Controllers;
using ASC.Web.Models;
using ASC.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ASC.Tests.Controllers;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _logger;
    private readonly Mock<IOptions<ApplicationSettings>> _settings;

    public HomeControllerTests()
    {
        _logger = new Mock<ILogger<HomeController>>();
        _settings = new Mock<IOptions<ApplicationSettings>>();
        _settings.Setup(x => x.Value).Returns(new ApplicationSettings
        {
            ApplicationTitle = "Automobile Service Center"
        });
    }

    [Fact]
    public void HomeController_Index_ValidRequest_ReturnsViewResult()
    {
        var controller = BuildController();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void HomeController_Index_ValidRequest_SessionContainsApplicationSettings()
    {
        var controller = BuildController();

        controller.Index();

        var data = controller.HttpContext.Session.GetObject<ApplicationSettings>("AppSettings");
        Assert.NotNull(data);
        Assert.Equal("Automobile Service Center", data!.ApplicationTitle);
    }

    [Fact]
    public void HomeController_Privacy_ValidRequest_ReturnsViewResult()
    {
        var controller = BuildController();

        var result = controller.Privacy();

        Assert.IsType<ViewResult>(result);
    }

    private HomeController BuildController()
    {
        var context = new DefaultHttpContext();
        context.Session = new FakeSession();

        return new HomeController(_logger.Object, _settings.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = context
            }
        };
    }
}
