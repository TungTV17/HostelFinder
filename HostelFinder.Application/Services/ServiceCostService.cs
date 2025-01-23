using AutoMapper;
using HostelFinder.Application.DTOs.ServiceCost.Request;
using HostelFinder.Application.DTOs.ServiceCost.Responses;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq.Expressions;

namespace HostelFinder.Application.Services
{
    public class ServiceCostService : IServiceCostService
    {
        private readonly IServiceCostRepository _serviceCostRepository;
        private readonly IMapper _mapper;
        private readonly IHostelRepository _hostelRepository;
        private readonly IServiceRepository _serviceRepository;
        public ServiceCostService(IServiceCostRepository serviceCostRepository, IMapper mapper, IHostelRepository hostelRepository, IServiceRepository serviceRepository)
        {
            _serviceCostRepository = serviceCostRepository;
            _mapper = mapper;
            _hostelRepository = hostelRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<Response<List<ServiceCostResponseDto>>> GetAllAsync()
        {
            var serviceCosts = await _serviceCostRepository.GetAllServiceCostListAsync();
            var result = _mapper.Map<List<ServiceCostResponseDto>>(serviceCosts);
            return new Response<List<ServiceCostResponseDto>>(result);
        }

        public async Task<Response<ServiceCostResponseDto>> GetByIdAsync(Guid id)
        {
            var serviceCost = await _serviceCostRepository.GetByIdAsync(id);
            if (serviceCost == null)
                return new Response<ServiceCostResponseDto>("Service cost not found.");

            var result = _mapper.Map<ServiceCostResponseDto>(serviceCost);
            return new Response<ServiceCostResponseDto>(result);
        }


        public async Task<Response<ServiceCostResponseDto>> UpdateAsync(Guid id, UpdateServiceCostDto serviceCostDto)
        {
            try
            {
                var serviceCost = await _serviceCostRepository.GetServiceCostById(id);
                if (serviceCost == null)
                    return new Response<ServiceCostResponseDto>("Service cost not found.");
                _mapper.Map(serviceCostDto, serviceCost);
                serviceCost = await _serviceCostRepository.UpdateAsync(serviceCost);

                var result = _mapper.Map<ServiceCostResponseDto>(serviceCost);
                return new Response<ServiceCostResponseDto>(result, "Cập nhật giá dịch vụ thành công.");
            }
            catch (Exception ex)
            {
                return new Response<ServiceCostResponseDto> { Message = ex.Message };
            }
        }

        public async Task<Response<bool>> DeleteAsync(Guid id)
        {
            var serviceCost = await _serviceCostRepository.GetByIdAsync(id);
            if (serviceCost == null)
                return new Response<bool>("Service cost not found.");

            await _serviceCostRepository.DeletePermanentAsync(id);
            return new Response<bool>(true, "Service cost deleted successfully.");
        }

        public async Task<Response<ServiceCostResponseDto>> CreateServiceCost(CreateServiceCostDto request)
        {
            try
            {
                //Check hostel exist
                var hostel = await _hostelRepository.GetByIdAsync(request.HostelId);
                if (hostel == null)
                {
                    return new Response<ServiceCostResponseDto> { Succeeded = false, Message = "Nhà trọ không tồn tại " };
                }

                //Check service exist 
                var service = await _serviceRepository.GetServiceByIdAsync(request.ServiceId);
                if (service == null)
                {
                    return new Response<ServiceCostResponseDto> { Succeeded = false, Message = "Dịch vụ không tồn tại" };
                }

                var overlappingServiceCost  = await _serviceCostRepository.GetOverlappingServiceCostAsync(request.HostelId, request.ServiceId, request.EffectiveFrom);

                if (overlappingServiceCost != null)
                {
                    return new Response<ServiceCostResponseDto> { Succeeded = false, Message = $"Đã tồn tại bảng giá dịch vụ cho dịch vụ này tại {hostel.HostelName} vào thời điểm này." };
                }
                
                //update effeactiveTo of the last service cost
                var lastServiceCost = await _serviceCostRepository.GetLastServiceCostAsync(request.HostelId, request.ServiceId);
                
                if (lastServiceCost != null && lastServiceCost.EffectiveTo == null)
                {
                    lastServiceCost.EffectiveTo = request.EffectiveFrom.AddDays(-1);
                    await _serviceCostRepository.UpdateAsync(lastServiceCost);
                }
                //map request to domain 
                var serviceCostDomain = _mapper.Map<ServiceCost>(request);

                serviceCostDomain.EffectiveTo = null;
                serviceCostDomain.CreatedOn = DateTime.Now;
                var serviceCostCreated = await _serviceCostRepository.AddAsync(serviceCostDomain);


                //map to Dto
                var serviceCostDto = _mapper.Map<ServiceCostResponseDto>(serviceCostCreated);

                return new Response<ServiceCostResponseDto> { Data = serviceCostDto, Succeeded = true, Message = $"Thêm giá dịch vụ {serviceCostDto.ServiceName} thành công " };


            }
            catch (Exception ex)
            {
                
                return new Response<ServiceCostResponseDto> { Succeeded = false, Errors = { ex.Message } };
            }

        }
        /// <summary>
        /// lấy ra giá tiền dịch vụ của 1 phòng trọ 
        /// </summary>
        /// <param name="hostelId"></param>
        /// <returns></returns>
        public async Task<Response<List<ServiceCostResponseDto>>> GetAllServiceCostByHostel(Guid hostelId)
        {
            Expression<Func<ServiceCost, bool>> filter = sr => sr.HostelId == hostelId;

            var servicerCosts = await _serviceCostRepository.GetAllServiceCostListWithConditionAsync(filter);
       
            if (servicerCosts == null || !servicerCosts.Any())
            {
                return new Response<List<ServiceCostResponseDto>>
                {
                    Succeeded = false,
                    Message = "Không tìm thấy giá của dịch vụ trong nhà trọ",
                    Data = null
                };
            }

            var responseDtos = _mapper.Map<List<ServiceCostResponseDto>>(servicerCosts);
            return new Response<List<ServiceCostResponseDto>> { Data = responseDtos, Succeeded = true, Message = "Lấy danh sách dịch vụ giá trọ thành công" };
        }
    }
}
