using HostelFinder.Application.DTOs.MaintenanceRecord.Request;
using HostelFinder.Application.DTOs.MaintenanceRecord.Response;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices;

public interface IMaintenanceRecordService
{
    Task<Response<bool>> AddMaintenanceRecordAsync(CreateMaintenanceRecordRequest request);
    Task<PagedResponse<List<ListMaintenanceRecordResponseDto>>> GetAllMaintenanceRecordAsync(GetAllMaintenanceRecordQuery request);
    Task<Response<bool>> DeleteMaintenanceRecordAsync(Guid id);
    Task<Response<bool>> EditMaintenanceRecordAsync(Guid id, EditMaintenanceRecordDto dto);
    Task<Response<MaintenanceRecordDetailsDto>> GetMaintenanceRecordByIdAsync(Guid id);
}