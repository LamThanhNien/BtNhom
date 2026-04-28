namespace ASC.Web.Areas.ServiceNotifications.Models;

public class NotificationDashboardViewModel
{
    public List<ServiceNotificationViewModel> Notifications { get; set; } = new();
}

public class ServiceNotificationViewModel
{
    public string RowKey { get; set; } = string.Empty;

    public string PartitionKey { get; set; } = string.Empty;

    public string VehicleName { get; set; } = string.Empty;

    public string VehicleType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string ServiceEngineer { get; set; } = string.Empty;

    public DateTime? RequestedDate { get; set; }

    public DateTime? NotificationDate { get; set; }

    public string Message { get; set; } = string.Empty;
}
