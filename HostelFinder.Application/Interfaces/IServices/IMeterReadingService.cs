using HostelFinder.Application.DTOs.MeterReading.Request;
using HostelFinder.Application.DTOs.MeterReading.Response;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IMeterReadingService
    {
        Task<Response<string>> AddMeterReadingAsync(Guid roomId, Guid serviceId, int? previousReading, int currentReading, int billingMonth, int billingYear);
        Task<Response<string>> AddMeterReadingListAsync(List<CreateMeterReadingDto> createMeterReadingDtos);
        Task<Response<List<ServiceCostReadingResponse>>> GetServiceCostReadingAsync(Guid hostelId, Guid roomId, int? billingMonth, int? billingYear);
        Task<PagedResponse<List<MeterReadingDto>>> GetPagedMeterReadingsAsync(int pageIndex, int pageSize, Guid hostelId, string? roomName = null, int? month = null, int? year = null);
        Task<Response<bool>> DeleteMeterReadingAsync(Guid id);
        Task<Response<bool>> EditMeterReadingAsync(Guid id, EditMeterReadingDto dto);
    }
}
