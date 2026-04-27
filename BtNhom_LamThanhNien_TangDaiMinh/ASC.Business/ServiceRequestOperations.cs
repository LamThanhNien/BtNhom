using ASC.Business.Interfaces;
using ASC.DataAccess;
using ASC.Model;

namespace ASC.Business
{
    public class ServiceRequestOperations : IServiceRequestOperations
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceRequestOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateServiceRequestAsync(ServiceRequest request)
        {
            await _unitOfWork.Repository<ServiceRequest>().AddAsync(request);
            await _unitOfWork.CommitAsync();
        }

        public async Task<ServiceRequest?> GetServiceRequestAsync(string rowKey, string partitionKey)
        {
            if (string.IsNullOrWhiteSpace(rowKey) || string.IsNullOrWhiteSpace(partitionKey))
            {
                return null;
            }

            return await _unitOfWork.Repository<ServiceRequest>()
                .FindAsync(x => x.Id == rowKey && x.PartitionKey == partitionKey);
        }

        public ServiceRequest UpdateServiceRequest(ServiceRequest request)
        {
            _unitOfWork.Repository<ServiceRequest>().UpdateAsync(request).GetAwaiter().GetResult();
            _unitOfWork.CommitAsync().GetAwaiter().GetResult();
            return request;
        }

        public async Task<ServiceRequest> AssignServiceEngineerAsync(string rowKey, string partitionKey, string serviceEngineerEmail)
        {
            if (string.IsNullOrWhiteSpace(serviceEngineerEmail))
            {
                throw new ArgumentException("Service engineer email is required.", nameof(serviceEngineerEmail));
            }

            var serviceRequest = await GetServiceRequestAsync(rowKey, partitionKey);
            if (serviceRequest is null)
            {
                throw new NullReferenceException();
            }

            serviceRequest.ServiceEngineer = serviceEngineerEmail;
            serviceRequest.Status = ASC.Model.Status.Initiated.ToString();
            serviceRequest.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<ServiceRequest>().UpdateAsync(serviceRequest);
            await _unitOfWork.CommitAsync();
            return serviceRequest;
        }

        public async Task<ServiceRequest> UpdateServiceRequestStatusAsync(string rowKey, string partitionKey, string status)
        {
            var serviceRequest = await _unitOfWork.Repository<ServiceRequest>()
                .FindAsync(x => x.Id == rowKey && x.PartitionKey == partitionKey);
            if (serviceRequest == null)
            {
                throw new NullReferenceException();
            }

            serviceRequest.Status = status;
            serviceRequest.UpdatedDate = DateTime.UtcNow;

            if (string.Equals(status, ASC.Model.Status.Completed.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                serviceRequest.CompletedDate = DateTime.UtcNow;
            }

            await _unitOfWork.Repository<ServiceRequest>().UpdateAsync(serviceRequest);
            await _unitOfWork.CommitAsync();
            return serviceRequest;
        }

        public async Task<List<ServiceRequest>> GetServiceRequestsByRequestedDateAndStatus(
            DateTime? requestedDate,
            List<string>? status = null,
            string email = "",
            string serviceEngineerEmail = "")
        {
            var query = Queries.GetDashboardQuery(requestedDate, status, email, serviceEngineerEmail);
            var serviceRequests = await _unitOfWork.Repository<ServiceRequest>()
                .FindAllByQuery(query);
            return serviceRequests.ToList();
        }
    }
}
