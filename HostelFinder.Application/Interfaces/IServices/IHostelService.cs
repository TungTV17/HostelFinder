using HostelFinder.Application.DTOs.Hostel.Requests;
using HostelFinder.Application.DTOs.Hostel.Responses;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Http;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Application.DTOs.Report;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IHostelService
    {
        Task<Response<HostelResponseDto>> AddHostelAsync(AddHostelRequestDto hostelDto, string imageUrl);
        Task<Response<bool>> DeleteHostelAsync(Guid hostelId);
        Task<PagedResponse<List<ListHostelResponseDto>>> GetHostelsByUserIdAsync(Guid landlordId, string? searchPhrase, int? pageNumber, int? pageSize, string? sortBy, SortDirection? sortDirection);
        Task<Response<HostelResponseDto>> GetHostelByIdAsync(Guid hostelId);
        Task<PagedResponse<List<ListHostelResponseDto>>> GetAllHostelAsync(GetAllHostelQuery request);
        Task<Response<HostelResponseDto>> UpdateHostelAsync(Guid hostelId, UpdateHostelRequestDto request, IFormFile image);
        Task<DashboardForLandlordResponse> GetDashboardDataAsync(Guid landlordId);

    }
}