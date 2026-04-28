using ASC.Business.Interfaces;
using ASC.Model;
using ASC.Web.Areas.ServiceRequests.Models;
using ASC.Web.Controllers;
using ASC.Web.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Areas.ServiceRequests.Controllers;

[Area("ServiceRequests")]
[Authorize(Roles = "Admin,Engineer")]
public class ServiceRequestController : BaseController
{
    private readonly IServiceRequestOperations _serviceRequestOperations;
    private readonly IMapper _mapper;
    private readonly IMasterDataCacheOperations _masterData;
    private readonly UserManager<IdentityUser> _userManager;

    public ServiceRequestController(
        IServiceRequestOperations operations,
        IMapper mapper,
        IMasterDataCacheOperations masterData,
        UserManager<IdentityUser> userManager)
    {
        _serviceRequestOperations = operations;
        _mapper = mapper;
        _masterData = masterData;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> ServiceRequest()
    {
        await PopulateVehicleSelectionsAsync();
        var model = new NewServiceRequestViewModel();
        if (HttpContext.User.IsInRole(Constants.CustomerRole))
        {
            var currentUser = HttpContext.User.GetCurrentUserDetails();
            model.CustomerEmail = !string.IsNullOrWhiteSpace(currentUser.Email) ? currentUser.Email : currentUser.UserName;
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ServiceRequest(NewServiceRequestViewModel request)
    {
        if (!ModelState.IsValid)
        {
            await PopulateVehicleSelectionsAsync();
            return View(request);
        }

        var currentUser = HttpContext.User.GetCurrentUserDetails();
        var currentUserEmail = !string.IsNullOrWhiteSpace(currentUser.Email) ? currentUser.Email : currentUser.UserName;

        if (string.IsNullOrWhiteSpace(currentUserEmail))
        {
            ModelState.AddModelError(string.Empty, "Unable to resolve current user email.");
            await PopulateVehicleSelectionsAsync();
            return View(request);
        }

        var serviceRequest = _mapper.Map<NewServiceRequestViewModel, ServiceRequest>(request);
        serviceRequest.RowKey = Guid.NewGuid().ToString();
        serviceRequest.PartitionKey = request.CustomerEmail;
        serviceRequest.RequestedDate = request.RequestedDate;
        serviceRequest.Status = ASC.Model.Status.New.ToString();
        serviceRequest.ServiceEngineer = string.Empty;
        serviceRequest.CreatedBy = currentUserEmail;
        serviceRequest.CreatedDate = DateTime.UtcNow;
        serviceRequest.UpdatedBy = currentUserEmail;
        serviceRequest.UpdatedDate = DateTime.UtcNow;

        await _serviceRequestOperations.CreateServiceRequestAsync(serviceRequest);
        return RedirectToAction("Dashboard", "Dashboard", new { Area = "ServiceRequests" });
    }

    private async Task PopulateVehicleSelectionsAsync()
    {
        var masterData = await _masterData.GetMasterDataCacheAsync();

        ViewBag.VehicleTypes = masterData.Values
            .Where(v => string.Equals(v.PartitionKey, MasterKeys.VehicleType.ToString(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        ViewBag.VehicleNames = masterData.Values
            .Where(v => string.Equals(v.PartitionKey, MasterKeys.VehicleName.ToString(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        var currentUser = HttpContext.User.GetCurrentUserDetails();
        var currentUserEmail = !string.IsNullOrWhiteSpace(currentUser.Email) ? currentUser.Email : currentUser.UserName;

        if (HttpContext.User.IsInRole(Constants.AdminRole) || HttpContext.User.IsInRole(Constants.ServiceEngineerRole))
        {
            var customers = await _userManager.GetUsersInRoleAsync(Constants.CustomerRole);
            ViewBag.CustomerEmails = customers.Select(p => p.Email ?? p.UserName).ToList();
        }
        else
        {
            ViewBag.CustomerEmails = new List<string?> { currentUserEmail };
        }
    }
}
