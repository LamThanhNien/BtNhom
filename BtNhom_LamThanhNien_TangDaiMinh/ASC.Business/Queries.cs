using System.Linq.Expressions;
using ASC.Model;

namespace ASC.Business
{
    public static class Queries
    {
        public static Expression<Func<ServiceRequest, bool>> GetDashboardQuery(
            DateTime? requestedDate,
            List<string>? status = null,
            string email = "",
            string serviceEngineerEmail = "")
        {
            var query = (Expression<Func<ServiceRequest, bool>>)(u => true);

            if (requestedDate.HasValue)
            {
                var requestedDateFilter = (Expression<Func<ServiceRequest, bool>>)(u =>
                    u.RequestedDate.HasValue && u.RequestedDate.Value >= requestedDate.Value);
                query = query.And(requestedDateFilter);
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailFilter = (Expression<Func<ServiceRequest, bool>>)(u => u.PartitionKey == email);
                query = query.And(emailFilter);
            }

            if (!string.IsNullOrWhiteSpace(serviceEngineerEmail))
            {
                var serviceEngineerFilter = (Expression<Func<ServiceRequest, bool>>)(u =>
                    (u.ServiceEngineer ?? string.Empty) == serviceEngineerEmail);
                query = query.And(serviceEngineerFilter);
            }

            if (status != null && status.Count > 0)
            {
                var normalizedStatuses = status
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Replace(" ", string.Empty).ToUpper())
                    .Distinct()
                    .ToList();

                if (normalizedStatuses.Count > 0)
                {
                    var statusQueries = (Expression<Func<ServiceRequest, bool>>)(u => false);
                    foreach (var state in normalizedStatuses)
                    {
                        var statusFilter = (Expression<Func<ServiceRequest, bool>>)(u =>
                            (u.Status ?? string.Empty).Replace(" ", string.Empty).ToUpper() == state);
                        statusQueries = statusQueries.Or(statusFilter);
                    }

                    query = query.And(statusQueries);
                }
            }

            return query;
        }
    }
}
