using AutoMapper;
using HostelFinder.Application.DTOs.MaintenanceRecord.Request;
using HostelFinder.Application.DTOs.MaintenanceRecord.Response;
using HostelFinder.Application.Helpers;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Services;

public class MaintenanceRecordService : IMaintenanceRecordService
{
    private readonly IMaintenanceRecordRepository _maintenanceRecordRepository;
    private readonly IMapper _mapper;

    public MaintenanceRecordService(IMaintenanceRecordRepository maintenanceRecordRepository,
        IMapper mapper)
    {
        _maintenanceRecordRepository = maintenanceRecordRepository;
        _mapper = mapper;
    }
    public async Task<Response<bool>> AddMaintenanceRecordAsync(CreateMaintenanceRecordRequest request)
    {
        try
        {
            var maintenanceRecord = _mapper.Map<MaintenanceRecord>(request);
            maintenanceRecord.CreatedOn = DateTime.Now;
            await _maintenanceRecordRepository.AddAsync(maintenanceRecord);
            return new Response<bool>() { Succeeded = true, Message = "Thêm lịch sử sửa chữa hoặc bảo dưỡng thành công" };
        }
        catch (Exception ex)
        {
            return new Response<bool>() { Succeeded = false, Message = ex.Message };
        }
    }

    public async Task<PagedResponse<List<ListMaintenanceRecordResponseDto>>> GetAllMaintenanceRecordAsync(GetAllMaintenanceRecordQuery request)
    {
        try
        {
            if (request.PageSize == 0)
            {
                request.PageSize = 10;
            }
            if (request.PageNumber == 0)
            {
                request.PageNumber = 1;
            }

            var maintenanceRecords = await _maintenanceRecordRepository.GetAllMatchingInMaintenanceRecordAsync(
                request.HostelId,
                request.SearchPhrase,
                request.PageSize ?? 10,
                request.PageNumber ?? 1,
                request.SortBy,
                request.SortDirection
            );

            var maintenanceRecordResponse = _mapper.Map<List<ListMaintenanceRecordResponseDto>>(maintenanceRecords.Data);
            var pageResponse = PaginationHelper.CreatePagedResponse(maintenanceRecordResponse, request.PageNumber ?? 1,
                request.PageSize ?? 10, maintenanceRecords.TotalRecords);
            return pageResponse;
        }
        catch (Exception ex)
        {
            return new PagedResponse<List<ListMaintenanceRecordResponseDto>>()
            {
                Succeeded = false,
                Message = ex.Message
            };
        }
    }

    public async Task<Response<bool>> DeleteMaintenanceRecordAsync(Guid id)
    {
        var existingRecord = await _maintenanceRecordRepository.GetByIdAsync(id);

        if (existingRecord == null)
        {
            return new Response<bool>("Maintenance record not found.");
        }

        await _maintenanceRecordRepository.DeleteAsync(id);

        return new Response<bool>(true, "Xóa thông tin sữa chữa & bảo dưỡng thành công.");
    }

    public async Task<Response<bool>> EditMaintenanceRecordAsync(Guid id, EditMaintenanceRecordDto dto)
    {
        var existingRecord = await _maintenanceRecordRepository.GetByIdAsync(id);

        if (existingRecord == null)
        {
            return new Response<bool>("Maintenance record not found.");
        }

        _mapper.Map(dto, existingRecord);

        await _maintenanceRecordRepository.UpdateAsync(existingRecord);

        return new Response<bool>(true, "Cập nhật thông tin sữa chữa & bảo dưỡng thành công.");
    }

    public async Task<Response<MaintenanceRecordDetailsDto>> GetMaintenanceRecordByIdAsync(Guid id)
    {
        var maintenanceRecord = await _maintenanceRecordRepository.GetByIdWithDetailsAsync(id);

        if (maintenanceRecord == null)
        {
            return new Response<MaintenanceRecordDetailsDto>("Maintenance record not found.");
        }

        var maintenanceRecordDto = _mapper.Map<MaintenanceRecordDetailsDto>(maintenanceRecord);

        return new Response<MaintenanceRecordDetailsDto>(maintenanceRecordDto, "Maintenance record retrieved successfully.");
    }

}