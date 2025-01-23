using HostelFinder.Application.Common;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IRentalContractRepository : IBaseGenericRepository<RentalContract>
    {
        Task<RentalContract?> GetRoomRentalContrctByRoom(Guid roomId);
        /// <summary>
        /// Method trả về hợp đồng hợp lệ ngoài khoảng thời gian đang cho thuê hiện tại
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<RentalContract?> CheckExpiredContractAsync(Guid roomId,DateTime startDate, DateTime? endDate);
        /// <summary>
        /// Pagaing and filter
        /// </summary>
        /// <param name="hostelId"></param>
        /// <param name="searchPhrase"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        Task<(List<RentalContract> rentalContracts, int  totalRecord)> GetAllMatchingRentalContractAysnc(Guid hostelId, string? searchPhrase,string? statusFilter, int pageNumber, int pageSize, string? sortBy, SortDirection sortDirection);
        Task<RentalContract> GetActiveRentalContractAsync(Guid roomId);
        Task<RentalContract> GetRentalContractByRoomIdAsync(Guid roomId);
    }
}
