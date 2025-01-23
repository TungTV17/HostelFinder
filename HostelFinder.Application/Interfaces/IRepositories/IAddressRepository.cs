using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IAddressRepository : IBaseGenericRepository<Address>
    {
        Task<Address> GetAddressByHostelIdAsync(Guid hostelId);
    }
}
