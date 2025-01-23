using AutoMapper;
using HostelFinder.Application.DTOs.Vehicle.Request;
using HostelFinder.Application.DTOs.Vehicle.Responses;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;
        private readonly ITenantRepository _tenantRepository;

        public VehicleService(IVehicleRepository vehicleRepository,
            IS3Service s3Service,
            IMapper mapper,
            ITenantRepository tenantRepository)
        {
            _vehicleRepository = vehicleRepository;
            _s3Service = s3Service;
            _mapper = mapper;
            _tenantRepository = tenantRepository;
        }

        public async Task<Response<IEnumerable<VehicleResponseDto>>> GetVehicleByTenantAsync(Guid tenantId)
        {
            try
            {
                var vehicles = await _vehicleRepository.GetByTenantAsync(tenantId);
                if (vehicles == null || !vehicles.Any())
                {
                    return new Response<IEnumerable<VehicleResponseDto>> { Succeeded = false, Message = "Không tìm thấy xe cho người thuê trọ" };
                }

                var vehicleDtos = _mapper.Map<IEnumerable<VehicleResponseDto>>(vehicles);
                return new Response<IEnumerable<VehicleResponseDto>> { Data = vehicleDtos, Succeeded = true };
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<VehicleResponseDto>> { Errors = new List<string>() { ex.Message } };
            }
        }

        public async Task<Response<VehicleResponseDto>> AddVehicleAsync(AddVehicleDto request)
        {
            try
            {
                var tenant = await _tenantRepository.GetByIdAsync(request.TenantId);
                if (tenant == null)
                {
                    return new Response<VehicleResponseDto> { Succeeded = false, Message = "Không tìm thấy người thuê trọ" };
                }

                //upload image vehical to aws
                var uploadVehicalImageToAWS = await _s3Service.UploadFileAsync(request.Image);

                var hehicle = _mapper.Map<Vehicle>(request);
                hehicle.ImageUrl = uploadVehicalImageToAWS;

                var createVehicalOfTerant = await _vehicleRepository.AddAsync(hehicle);
                var vehicalDto = _mapper.Map<VehicleResponseDto>(createVehicalOfTerant);

                return new Response<VehicleResponseDto> { Data = vehicalDto, Succeeded = true, Message = $"Thêm xe của người thuê  {tenant.FullName}" };
            }
            catch (Exception ex)
            {
                return new Response<VehicleResponseDto> { Errors = new List<string>() { ex.Message } };
            }

        }

        // Get vehicle by ID
        public async Task<Response<VehicleResponseDto>> GetVehicleByIdAsync(Guid vehicleId)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
                if (vehicle == null)
                {
                    return new Response<VehicleResponseDto> { Succeeded = false, Message = "Không tìm thấy xe" };
                }

                var vehicleDto = _mapper.Map<VehicleResponseDto>(vehicle);
                return new Response<VehicleResponseDto> { Data = vehicleDto, Succeeded = true };
            }   
            catch (Exception ex)
            {
                return new Response<VehicleResponseDto> { Errors = new List<string>() { ex.Message } };
            }
        }

        // Get all vehicles
        public async Task<Response<IEnumerable<VehicleResponseDto>>> GetAllVehiclesAsync()
        {
            try
            {
                var vehicles = await _vehicleRepository.ListAllAsync();
                var vehiclesDto = _mapper.Map<IEnumerable<VehicleResponseDto>>(vehicles);

                return new Response<IEnumerable<VehicleResponseDto>> { Data = vehiclesDto, Succeeded = true };
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<VehicleResponseDto>> { Errors = new List<string>() { ex.Message } };
            }
        }

        // Update vehicle
        public async Task<Response<VehicleResponseDto>> UpdateVehicleAsync(Guid vehicleId, AddVehicleDto request)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
                if (vehicle == null)
                {
                    return new Response<VehicleResponseDto> { Succeeded = false, Message = "Không tìm thấy xe" };
                }

                // If new image is uploaded, delete the old image from AWS
                if (request.Image != null)
                {
                    if (!string.IsNullOrEmpty(vehicle.ImageUrl))
                    {
                        await _s3Service.DeleteFileAsync(vehicle.ImageUrl);
                    }

                    // Upload the new image to AWS
                    vehicle.ImageUrl = await _s3Service.UploadFileAsync(request.Image);
                }

                // Map and update vehicle
                _mapper.Map(request, vehicle);
                var updatedVehicle = await _vehicleRepository.UpdateAsync(vehicle);
                var vehicleDto = _mapper.Map<VehicleResponseDto>(updatedVehicle);

                return new Response<VehicleResponseDto> { Data = vehicleDto, Succeeded = true, Message = "Cập nhật xe thành công" };
            }
            catch (Exception ex)
            {
                return new Response<VehicleResponseDto> { Errors = new List<string>() { ex.Message } };
            }
        }

        // Delete vehicle
        public async Task<Response<bool>> DeleteVehicleAsync(Guid vehicleId)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
                if (vehicle == null)
                {
                    return new Response<bool> { Succeeded = false, Message = "Không tìm thấy xe" };
                }

                // Delete the vehicle image from AWS
                if (!string.IsNullOrEmpty(vehicle.ImageUrl))
                {
                    await _s3Service.DeleteFileAsync(vehicle.ImageUrl);
                }

                // Delete vehicle
                await _vehicleRepository.DeleteAsync(vehicleId);
                return new Response<bool> { Data = true, Succeeded = true, Message = "Xóa xe thành công" };
            }
            catch (Exception ex)
            {
                return new Response<bool> { Errors = new List<string>() { ex.Message } };
            }
        }
    }
}
