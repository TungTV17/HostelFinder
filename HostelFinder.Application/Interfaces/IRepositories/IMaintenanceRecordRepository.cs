using HostelFinder.Application.Common;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories;

public interface IMaintenanceRecordRepository : IBaseGenericRepository<MaintenanceRecord>
{
    Task<(IEnumerable<MaintenanceRecord> Data, int TotalRecords)> GetAllMatchingInMaintenanceRecordAsync(Guid hostelId,string? searchPhrase, int pageSize, int pageNumber, string? sortBy, SortDirection? sortDirection);
    
    /// <summary>
    /// lấy ra chi phí sửa chữa của một hostel trong một năm
    /// </summary>
    /// <param name="hostelId"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    Task<decimal> GetTotalCostOfMaintenanceRecordInYearAsync(Guid hostelId, int year);
    Task<MaintenanceRecord> GetByIdWithDetailsAsync(Guid id);
    Task<decimal> GetTotalCostOfMaintenanceRecordInMonthAsync(Guid hostelId, int year, int month);
}