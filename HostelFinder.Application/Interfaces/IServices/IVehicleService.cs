using HostelFinder.Application.DTOs.Vehicle.Request;
using HostelFinder.Application.DTOs.Vehicle.Responses;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IVehicleService
    {
        Task<Response<VehicleResponseDto>> AddVehicleAsync(AddVehicleDto request);
        Task<Response<VehicleResponseDto>> GetVehicleByIdAsync(Guid vehicleId); 
        Task<Response<IEnumerable<VehicleResponseDto>>> GetAllVehiclesAsync(); 
        Task<Response<VehicleResponseDto>> UpdateVehicleAsync(Guid vehicleId, AddVehicleDto request); 
        Task<Response<bool>> DeleteVehicleAsync(Guid vehicleId);
        Task<Response<IEnumerable<VehicleResponseDto>>> GetVehicleByTenantAsync(Guid tenantId);
    }
}
