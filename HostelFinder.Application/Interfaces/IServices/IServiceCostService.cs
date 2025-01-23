using HostelFinder.Application.DTOs.ServiceCost.Request;
using HostelFinder.Application.DTOs.ServiceCost.Responses;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IServiceCostService
    {
        Task<Response<List<ServiceCostResponseDto>>> GetAllAsync();
        Task<Response<ServiceCostResponseDto>> GetByIdAsync(Guid id);
        Task<Response<ServiceCostResponseDto>> CreateServiceCost(CreateServiceCostDto request);
        Task<Response<ServiceCostResponseDto>> UpdateAsync(Guid id, UpdateServiceCostDto serviceCostDto);
        Task<Response<bool>> DeleteAsync(Guid id);
        Task<Response<List<ServiceCostResponseDto>>> GetAllServiceCostByHostel(Guid hostelId);
    }
}
