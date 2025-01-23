using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IServiceRepository : IBaseGenericRepository<Service>
    {
        Task<bool> CheckDuplicateServiceAsync(string serviceName);

        Task<IEnumerable<Service>> GetAllServiceAsync();

        Task<Service> GetServiceByIdAsync(Guid serviceId);

        Task<IEnumerable<HostelServices>> GetServiceByHostelIdAsync(Guid hostelId);

        Task<ServiceCost> GetCurrentServiceCostAsync(Guid hostelId, Guid serviceId);
       
    }
}