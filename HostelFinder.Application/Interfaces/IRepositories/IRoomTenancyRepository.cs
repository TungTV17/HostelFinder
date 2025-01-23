using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IRoomTenancyRepository : IBaseGenericRepository<RoomTenancy>
    {
        Task<int> CountCurrentTenantsAsync(Guid roomId);
        /// <summary>
        /// lấy ra danh sách người đang thuê trọ hiện tại
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<List<RoomTenancy>> GetRoomTenacyByIdAsync(Guid roomId);

        /// <summary>
        /// lấy ra những người đang ở trong hợp đồng
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<List<RoomTenancy>> GetTenacyCurrentlyByRoom(Guid roomId, DateTime startDate, DateTime? endDate);
        Task<RoomTenancy?> GetRoomTenancyByTenantIdAsync(Guid tenantId);
        Task<RoomTenancy?> GetEarliestRoomTenancyByRoomIdAsync(Guid roomId);
        
        Task<int> CountCurrentTenantsByRoomsInMonthAsync(Guid roomId, int month, int year);
        
        // lấy ra người đại diện hợp đồng
        Task<RoomTenancy?> GetRoomTenancyRepresentativeAsync(Guid roomId);
    }
}
