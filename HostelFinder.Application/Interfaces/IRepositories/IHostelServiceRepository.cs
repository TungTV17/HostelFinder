using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IHostelServiceRepository : IBaseGenericRepository<HostelServices>
    {
        Task<List<HostelServices>> GetServicesByHostelIdAsync(Guid hostelId);
    }
}
