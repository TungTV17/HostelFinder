using HostelFinder.Application.DTOs.RentalContract.Request;
using HostelFinder.Application.DTOs.RentalContract.Response;
using HostelFinder.Application.DTOs.Room.Responses;
using HostelFinder.Application.DTOs.Tenancies.Requests;
using HostelFinder.Application.DTOs.Tenancies.Responses;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface ITenantService
    {
        Task<TenantResponse> AddTenentServiceAsync(AddTenantDto request);
        Task<List<InformationTenacyReponseDto>> GetInformationTenacyAsync(Guid roomId);
        Task<Response<string>> AddRoommateAsync(AddRoommateDto request);
        Task<PagedResponse<List<InformationTenanciesResponseDto>>> GetAllTenantsByHostelAsync(Guid hostelId, string? roomName, int pageNumber, int pageSize, string? status);
        Task<Response<string>> MoveOutAsync(Guid tenantId, Guid roomId);
        
        Task<Response<string>> UpdateTenantAsync(UpdateTenantDto request); 
        
        Task<Response<TenantResponseDto>> GetTenantByIdAsync(Guid tenantId);
    }
}
