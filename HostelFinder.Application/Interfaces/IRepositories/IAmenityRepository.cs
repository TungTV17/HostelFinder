using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories;

public interface IAmenityRepository : IBaseGenericRepository<Amenity>
{
    Task<Amenity> AddAsync(Amenity amenity);
    Task<List<Amenity>> ListAllAsync();
    Task<bool> ExistsByNameAsync(string amenityName);
    Task<IEnumerable<Amenity>> GetAmenitysByRoomIdAsync(Guid roomId);
}