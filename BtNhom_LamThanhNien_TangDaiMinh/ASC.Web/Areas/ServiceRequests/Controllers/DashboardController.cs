using ASC.Business.Interfaces;
using ASC.Model;
using ASC.Web.Areas.ServiceRequests.Models;
using ASC.Web.Controllers;
using ASC.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Areas.ServiceRequests.Controllers;

[Area("ServiceRequests")]
[Authorize(Roles = "Admin,Engineer,User")]
public class DashboardController : BaseController
{
    private readonly IServiceRequestOperations _serviceRequestOperations;
    private readonly IMasterDataCacheOperations _masterData;
    private readonly UserManager<IdentityUser> _userManager;

    public DashboardController(
        IServiceRequestOperations operations,
        IMasterDataCacheOperations masterData,
        UserManager<IdentityUser> userManager)
    {
        _serviceRequestOperations = operations;
        _masterData = masterData;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        await _masterData.GetMasterDataCacheAsync();

        var status = new List<string>
        {
            ASC.Model.Status.New.ToString(),
            ASC.Model.Status.InProgress.ToString(),
            ASC.Model.Status.Initiated.ToString(),
            ASC.Model.Status.RequestForInformation.ToString()
        };

        var currentUser = User.ToCurrentUser();
        var currentUserEmail = !string.IsNullOrWhiteSpace(currentUser.Email)
            ? currentUser.Email
            : currentUser.UserName;

        List<ServiceRequest> serviceRequests;
        if (HttpContext.User.IsInRole(Constants.AdminRole))
        {
            serviceRequests = await _serviceRequestOperations.GetServiceRequestsByRequestedDateAndStatus(
                DateTime.UtcNow.AddYears(-1),
                status);
        }
        else if (HttpContext.User.IsInRole(Constants.ServiceEngineerRole))
        {
            serviceRequests = await _serviceRequestOperations.GetServiceRequestsByRequestedDateAndStatus(
                DateTime.UtcNow.AddYears(-1),
                status,
                serviceEngineerEmail: currentUserEmail);
        }
        else
        {
            serviceRequests = await _serviceRequestOperations.GetServiceRequestsByRequestedDateAndStatus(
                DateTime.UtcNow.AddYears(-1),
                email: currentUserEmail);
        }

        var engineers = await _userManager.GetUsersInRoleAsync(Constants.ServiceEngineerRole);
        var engineerEmails = engineers
            .Where(e => e.Email != null)
            .Select(e => e.Email!)
            .OrderBy(e => e)
            .ToList();

        return View(new DashboardViewModel
        {
            ServiceRequests = serviceRequests
                .OrderByDescending(p => p.RequestedDate)
                .ToList(),
            ServiceEngineerEmails = engineerEmails
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Engineer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(string rowKey, string partitionKey, string status)
    {
        if (string.IsNullOrWhiteSpace(rowKey) || string.IsNullOrWhiteSpace(partitionKey))
        {
            return BadRequest();
        }

        if (!TryNormalizeStatus(status, out var normalizedStatus))
        {
            return BadRequest("Invalid service request status.");
        }

        var serviceRequest = await _serviceRequestOperations.GetServiceRequestAsync(rowKey, partitionKey);
        if (serviceRequest is null)
        {
            return NotFound();
        }

        var currentUser = User.ToCurrentUser();
        var currentUserEmail = !string.IsNullOrWhiteSpace(currentUser.Email)
            ? currentUser.Email
            : currentUser.UserName;

        if (HttpContext.User.IsInRole(Constants.ServiceEngineerRole))
        {
            if (string.IsNullOrWhiteSpace(currentUserEmail) ||
                !string.Equals(serviceRequest.ServiceEngineer, currentUserEmail, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }
        }

        await _serviceRequestOperations.UpdateServiceRequestStatusAsync(rowKey, partitionKey, normalizedStatus, currentUserEmail);
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignServiceEngineer(string rowKey, string partitionKey, string serviceEngineerEmail)
    {
        if (string.IsNullOrWhiteSpace(rowKey) ||
            string.IsNullOrWhiteSpace(partitionKey) ||
            string.IsNullOrWhiteSpace(serviceEngineerEmail))
        {
            return BadRequest();
        }

        var currentUser = User.ToCurrentUser();
        var currentUserEmail = !string.IsNullOrWhiteSpace(currentUser.Email)
            ? currentUser.Email
            : currentUser.UserName;

        await _serviceRequestOperations.AssignServiceEngineerAsync(rowKey, partitionKey, serviceEngineerEmail.Trim(), currentUserEmail);
        return RedirectToAction(nameof(Dashboard));
    }

    private static bool TryNormalizeStatus(string inputStatus, out string status)
    {
        status = string.Empty;
        if (string.IsNullOrWhiteSpace(inputStatus))
        {
            return false;
        }

        var normalizedInput = inputStatus.Replace(" ", string.Empty).Trim().ToUpperInvariant();
        foreach (var validStatus in Enum.GetNames(typeof(ASC.Model.Status)))
        {
            if (validStatus.Replace(" ", string.Empty).Trim().ToUpperInvariant() == normalizedInput)
            {
                status = validStatus;
                return true;
            }
        }

        return false;
    }
}
