using ASC.Model;

namespace ASC.Business.Interfaces
{
    public interface IServiceRequestOperations
    {
        Task CreateServiceRequestAsync(ServiceRequest request);
        Task<ServiceRequest?> GetServiceRequestAsync(string rowKey, string partitionKey);
        Task<ServiceRequest> UpdateServiceRequest(ServiceRequest request);
        Task<ServiceRequest> AssignServiceEngineerAsync(string rowKey, string partitionKey, string serviceEngineerEmail, string updatedBy);
        Task<ServiceRequest> UpdateServiceRequestStatusAsync(string rowKey, string partitionKey, string status, string updatedBy);
        Task<List<ServiceRequest>> GetServiceRequestsByRequestedDateAndStatus(
            DateTime? requestedDate,
            List<string>? status = null,
            string email = "",
            string serviceEngineerEmail = "");
    }
}
