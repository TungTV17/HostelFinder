using HostelFinder.Application.Common;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace HostelFinder.Application.Interfaces.IRepositories;

public interface IHostelRepository : IBaseGenericRepository<Hostel>
{
    Task<bool> CheckDuplicateHostelAsync(string hostelName, string province, string district, string commune, string detailAddress);
    Task<IEnumerable<Hostel>> GetHostelsByUserIdAsync(Guid landlordId);
    Task<Hostel> GetHostelWithReviewsByPostIdAsync(Guid postId);
    Task<Hostel?> GetHostelByIdAsync(Guid postId);
    Task<(IEnumerable<Hostel> Data, int TotalRecords)> GetAllMatchingAsync(string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection);
    Task<Hostel> GetHostelByIdAndUserIdAsync(Guid hostelId,Guid userId);
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task<(IEnumerable<Hostel> Data, int TotalRecords)> GetAllMatchingInLandLordAsync(Guid landlordId,string? searchPhrase, int? pageSize, int? pageNumber, string? sortBy, SortDirection? sortDirection);
    Task<int> GetHostelCountAsync(Guid landlordId);
    Task<int> GetTenantCountAsync(Guid landlordId);
    Task<int> GetRoomCountAsync(Guid landlordId);
    Task<int> GetOccupiedRoomCountAsync(Guid landlordId);
    Task<int> GetAvailableRoomCountAsync(Guid landlordId);
    Task<int> GetAllInvoicesCountAsync(Guid landlordId);
    Task<int> GetUnpaidInvoicesCountAsync(Guid landlordId);
    Task<int> GetExpiringContractsCountAsync(Guid landlordId, DateTime currentDate);
    Task<int> GetPostCountAsync(Guid landlordId);
}
