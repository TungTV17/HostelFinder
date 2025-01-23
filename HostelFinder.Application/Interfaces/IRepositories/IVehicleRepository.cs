using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IVehicleRepository : IBaseGenericRepository<Vehicle>
    {
        Task<IEnumerable<Vehicle>> GetByTenantAsync(Guid tenantId);
    }
}
