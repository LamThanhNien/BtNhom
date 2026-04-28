using ASC.Business.Interfaces;
using ASC.Model;
using ASC.Web.Areas.ServiceNotifications.Models;
using ASC.Web.Controllers;
using ASC.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Areas.ServiceNotifications.Controllers;

[Area("ServiceNotifications")]
[Authorize(Roles = "Admin,Engineer,User")]
public class NotificationsController : BaseController
{
    private readonly IServiceRequestOperations _serviceRequestOperations;

    public NotificationsController(IServiceRequestOperations serviceRequestOperations)
    {
        _serviceRequestOperations = serviceRequestOperations;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUser = User.ToCurrentUser();
        var currentUserEmail = !string.IsNullOrWhiteSpace(currentUser.Email)
            ? currentUser.Email
            : currentUser.UserName;

        List<ServiceRequest> serviceRequests;
        if (HttpContext.User.IsInRole(Constants.AdminRole))
        {
            serviceRequests = await _serviceRequestOperations.GetServiceRequestsByRequestedDateAndStatus(
                DateTime.UtcNow.AddDays(-30));
        }
        else if (HttpContext.User.IsInRole(Constants.ServiceEngineerRole))
        {
            serviceRequests = await _serviceRequestOperations.GetServiceRequestsByRequestedDateAndStatus(
                DateTime.UtcNow.AddDays(-30),
                serviceEngineerEmail: currentUserEmail);
        }
        else
        {
            serviceRequests = await _serviceRequestOperations.GetServiceRequestsByRequestedDateAndStatus(
                DateTime.UtcNow.AddDays(-30),
                email: currentUserEmail);
        }

        var notifications = serviceRequests
            .OrderByDescending(r => r.UpdatedDate ?? r.RequestedDate)
            .Select(r => new ServiceNotificationViewModel
            {
                RowKey = r.RowKey,
                PartitionKey = r.PartitionKey,
                VehicleName = r.VehicleName,
                VehicleType = r.VehicleType,
                Status = r.Status,
                ServiceEngineer = r.ServiceEngineer ?? string.Empty,
                RequestedDate = r.RequestedDate,
                NotificationDate = r.UpdatedDate ?? r.RequestedDate,
                Message = BuildNotificationMessage(r)
            })
            .ToList();

        return View(new NotificationDashboardViewModel
        {
            Notifications = notifications
        });
    }

    private static string BuildNotificationMessage(ServiceRequest request)
    {
        var status = string.IsNullOrWhiteSpace(request.Status) ? "Updated" : request.Status;
        var engineer = string.IsNullOrWhiteSpace(request.ServiceEngineer)
            ? "TBD"
            : request.ServiceEngineer;

        return $"Request status: {status}. Assigned engineer: {engineer}.";
    }
}
