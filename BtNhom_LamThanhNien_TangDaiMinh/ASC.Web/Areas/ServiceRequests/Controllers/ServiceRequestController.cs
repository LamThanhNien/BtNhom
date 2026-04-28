using ASC.Business.Interfaces;
using ASC.Model;
using ASC.Web.Areas.ServiceRequests.Models;
using ASC.Web.Controllers;
using ASC.Web.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Areas.ServiceRequests.Controllers;

[Area("ServiceRequests")]
[Authorize(Roles = "Admin,Engineer,User")]
public class ServiceRequestController : BaseController
{
    private readonly IServiceRequestOperations _serviceRequestOperations;
    private readonly IMapper _mapper;
    private readonly IMasterDataCacheOperations _masterData;

    public ServiceRequestController(
        IServiceRequestOperations operations,
        IMapper mapper,
        IMasterDataCacheOperations masterData)
    {
        _serviceRequestOperations = operations;
        _mapper = mapper;
        _masterData = masterData;
    }

    [HttpGet]
    public async Task<IActionResult> ServiceRequest()
    {
        await PopulateVehicleSelectionsAsync();
        return View(new NewServiceRequestViewModel());
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
        var currentUserEmail = !string.IsNullOrWhiteSpace(currentUser.Email)
            ? currentUser.Email
            : currentUser.UserName;

        if (string.IsNullOrWhiteSpace(currentUserEmail))
        {
            ModelState.AddModelError(string.Empty, "Unable to resolve current user email.");
            await PopulateVehicleSelectionsAsync();
            return View(request);
        }

        var serviceRequest = _mapper.Map<NewServiceRequestViewModel, ServiceRequest>(request);
        serviceRequest.RowKey = Guid.NewGuid().ToString();
        serviceRequest.PartitionKey = currentUserEmail;
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
    }
}
