using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IRoomAmentityRepository : IBaseGenericRepository<RoomAmenities>
    {
        Task DeleteByRoomIdAsync(Guid roomId);
        Task AddRangeAsync(List<RoomAmenities> amenitiesList);
        Task<List<RoomAmenities>> GetAmenitiesByRoomIdAsync(Guid roomId);
    }
}
