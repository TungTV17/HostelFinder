using AutoMapper;
using HostelFinder.Application.DTOs.Service.Request;
using HostelFinder.Application.DTOs.Service.Response;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IHostelServiceRepository _hostelServiceRepository;
        private readonly IMapper _mapper;

        public ServiceService(IServiceRepository serviceRepository, IMapper mapper, IHostelServiceRepository hostelServiceRepository)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
            _hostelServiceRepository = hostelServiceRepository;
        }

        public async Task<Response<List<ServiceResponseDTO>>> GetAllServicesAsync()
        {
            var services = await _serviceRepository.ListAllAsync();
            var serviceResponseDtos = _mapper.Map<List<ServiceResponseDTO>>(services);
            return new Response<List<ServiceResponseDTO>> { Succeeded = true, Data = serviceResponseDtos };
        }

        public async Task<Response<ServiceResponseDTO>> GetServiceByIdAsync(Guid id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return new Response<ServiceResponseDTO>("Service not found.");
            }
            var serviceResponseDto = _mapper.Map<ServiceResponseDTO>(service);
            return new Response<ServiceResponseDTO> { Succeeded = true, Data = serviceResponseDto };
        }

        public async Task<Response<ServiceResponseDTO>> AddServiceAsync(ServiceCreateRequestDTO serviceCreateRequestDTO)
        {
            var isDuplicate = await _serviceRepository.CheckDuplicateServiceAsync(serviceCreateRequestDTO.ServiceName);
            if (isDuplicate)
            {
                return new Response<ServiceResponseDTO>("Dịch vụ đã tồn tại.");
            }

            var service = _mapper.Map<Service>(serviceCreateRequestDTO);
            service.CreatedOn = DateTime.Now;
            service.CreatedBy = "System";

            try
            {
                await _serviceRepository.AddAsync(service);

                var hostelService = new HostelServices
                {
                    HostelId = serviceCreateRequestDTO.HostelId,
                    ServiceId = service.Id,
                    CreatedOn = DateTime.Now,
                    CreatedBy = "System"
                };

                // Add the HostelService to the corresponding repository
                await _hostelServiceRepository.AddAsync(hostelService);

                // Map the newly added service to the response DTO
                var serviceResponseDto = _mapper.Map<ServiceResponseDTO>(service);

                return new Response<ServiceResponseDTO>
                {
                    Succeeded = true,
                    Data = serviceResponseDto,
                    Message = $"Dịch vụ {serviceCreateRequestDTO.ServiceName} đã được thêm thành công."
                };
            }
            catch (Exception ex)
            {
                return new Response<ServiceResponseDTO>(message: ex.Message);
            }
        }

        public async Task<Response<ServiceResponseDTO>> UpdateServiceAsync(Guid id, ServiceUpdateRequestDTO serviceUpdateRequestDTO)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return new Response<ServiceResponseDTO>("Service not found.");
            }

            try
            {
                _mapper.Map(serviceUpdateRequestDTO, service);
                await _serviceRepository.UpdateAsync(service);
                var updatedServiceResponseDto = _mapper.Map<ServiceResponseDTO>(service);
                return new Response<ServiceResponseDTO> { Succeeded = true, Data = updatedServiceResponseDto, Message = "Service updated successfully." };
            }
            catch (Exception ex)
            {
                return new Response<ServiceResponseDTO>(message: ex.Message);
            }
        }

        public async Task<Response<string>> DeleteServiceAsync(Guid id)
        {
            try
            {
                var service = await _hostelServiceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    return new Response<string>("Service not found.") { Succeeded = false };
                }

                await _hostelServiceRepository.DeletePermanentAsync(id);
                return new Response<string>("Service deleted successfully.") { Succeeded = true };
            }
            catch (Exception ex)
            {
                return new Response<string>(message: ex.Message) { Succeeded = false };
            }

        }

        public async Task<Response<List<HostelServiceResponseDto>>> GetAllServiceByHostelAsync(Guid hostelId)
        {
            try
            {
                var hostelServices = await _serviceRepository.GetServiceByHostelIdAsync(hostelId);
                if (hostelServices == null)
                {
                    return new Response<List<HostelServiceResponseDto>> { Message = "Không tìm thấy dịch vụ nào trong phòng trọ", Succeeded = true, Data = new List<HostelServiceResponseDto>() };
                }

                var hostelServiceResponseDto = _mapper.Map<List<HostelServiceResponseDto>>(hostelServices);


                return new Response<List<HostelServiceResponseDto>> { Data = hostelServiceResponseDto, Succeeded = true, Message = "Lấy danh sách dịch vụ trong phòng trọ thành công" };
            }
            catch (Exception ex)
            {
                return new Response<List<HostelServiceResponseDto>> { Succeeded = false, Message = ex.Message };
            }

        } 
    }
}
