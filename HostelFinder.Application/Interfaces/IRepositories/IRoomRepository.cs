using HostelFinder.Application.Common;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IRoomRepository : IBaseGenericRepository<Room>
    {
        Task<bool> RoomExistsAsync(string roomName, Guid hostelId);
        Task<List<Room>> ListAllWithDetailsAsync();
        Task<Room> GetRoomWithDetailsAndServiceCostsByIdAsync(Guid roomId);
        Task<List<Room>> GetRoomsByHostelIdAsync(Guid hostelId, int? floor);
        Task<List<Room>> GetRoomsByHostelIdAsync(Guid hostelId);
        Task<Room> GetRoomByIdAsync(Guid roomId);
        Task<List<Room>> GetRoomsByHostelAsync(Guid hostelId);
        Task<(IEnumerable<Room> Data, int TotalRecords)> GetAllMatchingInLandLordAsync(Guid landlordId,string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection? sortDirection);

    }
}
