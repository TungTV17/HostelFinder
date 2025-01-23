using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IMeterReadingRepository : IBaseGenericRepository<MeterReading>
    {
        Task<MeterReading?> GetPreviousMeterReadingAsync(Guid roomId, Guid serviceId, int billingMonth, int billingYear);
        Task<MeterReading> GetCurrentMeterReadingAsync(Guid roomId, Guid serviceId, int billingMonth, int billingYear);
        Task<(List<MeterReading> Data, int TotalRecords)> GetPagedMeterReadingsAsync(int pageIndex, int pageSize, Guid hostelId, string? roomName = null, int? month = null, int? year = null);
    }
}
