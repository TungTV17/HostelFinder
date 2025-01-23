using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using HostelFinder.Application.DTOs.MeterReading.Request;
using HostelFinder.Application.DTOs.MeterReading.Response;
using HostelFinder.Application.Helpers;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.Services
{
    public class MeterReadingService : IMeterReadingService
    {
        private readonly IMeterReadingRepository _meterReadingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceCostService _serviceCostService;
        private readonly IMapper _mapper;
        private readonly IInVoiceRepository _inVoiceRepository;

        public MeterReadingService
        (
            IMeterReadingRepository meterReadingRepository,
            IRoomRepository roomRepository,
            IServiceRepository serviceRepository,
            IServiceCostService serviceCostService, IMapper mapper,
            IInVoiceRepository inVoiceRepository
            )
        {
            _meterReadingRepository = meterReadingRepository;
            _roomRepository = roomRepository;
            _serviceRepository = serviceRepository;
            _serviceCostService = serviceCostService;
            _mapper = mapper;
            _inVoiceRepository = inVoiceRepository;
        }
        public async Task<Response<string>> AddMeterReadingAsync(Guid roomId, Guid serviceId, int? previousReading, int currentReading, int billingMonth, int billingYear)
        {
            try
            {
                var existingRoom = await _roomRepository.GetRoomByIdAsync(roomId);
                if (existingRoom == null)
                {
                    return new Response<string> { Message = "Không tìm thấy phòng để ghi số liệu", Succeeded = false };
                }

                var existingService = await _serviceRepository.GetByIdAsync(serviceId);
                if (existingService == null)
                {
                    return new Response<string> { Succeeded = false, Message = "Không có dịch vụ trong phòng trọ" };
                }

                if (currentReading < 0 && previousReading < 0)
                {
                    return new Response<string> { Message = "Số liệu phải lớn hơn 0", Succeeded = false };
                }

                if (billingMonth < 1 || billingMonth > 12)
                {
                    return new Response<string> { Message = "Tháng lập hóa đơn phải nằm trong khoảng từ 1 đến 12.", Succeeded = false };
                }

                if (billingYear < 0)
                {
                    return new Response<string> { Message = $"Năm lập hóa đơn không hợp lệ. Phải lớn hơn 2000 và nhỏ hơn {DateTime.Now.Year}", Succeeded = false };
                }
                var existingReading = await _meterReadingRepository.GetCurrentMeterReadingAsync(roomId, serviceId, billingMonth, billingYear);

                if (existingReading != null)
                {
                    return new Response<string> { Message = "Đã có số liệu đọc cho phòng dịch và dịch vụ trong tháng này", Succeeded = false };
                }
                // lấy số liệu tháng trước
                var previousMeterReading = await _meterReadingRepository.GetPreviousMeterReadingAsync(roomId, serviceId, billingMonth, billingYear);
                //nếu không có số liệu tháng trước thì sẽ mặc định là 0
                if (previousMeterReading == null)
                {
                    var newPreviousMeterReading = new MeterReading
                    {
                        RoomId = roomId,
                        ServiceId = serviceId,
                        Reading = previousReading ?? 0,
                        BillingMonth = billingMonth == 1 ? 12 : billingMonth - 1,
                        BillingYear = billingMonth == 1 ? billingYear - 1 : billingYear,
                        CreatedOn = DateTime.Now,
                        IsDeleted = false
                    };
                    await _meterReadingRepository.AddAsync(newPreviousMeterReading);

                }
                if ((previousMeterReading?.Reading ?? 0) > currentReading)
                {
                    return new Response<string> { Message = $"Số liệu tháng {billingMonth} phải lớn hơn hoặc bằng số liệu tháng {billingMonth - 1}", Succeeded = false };
                }
                var meterReading = new MeterReading
                {
                    Id = Guid.NewGuid(),
                    RoomId = roomId,
                    ServiceId = serviceId,
                    Reading = currentReading,
                    BillingMonth = billingMonth,
                    BillingYear = billingYear,
                    CreatedOn = DateTime.Now,
                    IsDeleted = false,
                };

                await _meterReadingRepository.AddAsync(meterReading);


                return new Response<string> { Message = $"Ghi thành công {existingService.ServiceName} của phòng {existingRoom.RoomName} ở tháng {billingMonth}/{billingYear}", Succeeded = true };
            }
            catch (Exception ex)
            {
                return new Response<string> { Message = ex.Message, Succeeded = false };
            }
        }

        public async Task<Response<string>> AddMeterReadingListAsync(List<CreateMeterReadingDto> createMeterReadingDtos)
        {
            try
            {
                foreach (var meterReading in createMeterReadingDtos.ToList())
                {
                    if (meterReading.previousReading.HasValue && meterReading.currentReading < meterReading.previousReading)
                    {
                        return new Response<string> { Message = "Số liệu tháng này phải lớn hơn hoặc bằng số liệu tháng trước", Succeeded = false };
                    }
                    var service = await _serviceRepository.GetByIdAsync(meterReading.serviceId);
                    if (service.ChargingMethod != Domain.Enums.ChargingMethod.PerUsageUnit)
                    {
                        return new Response<string> { Message = "Dịch vụ này không phải là dịch vụ đo số liệu", Succeeded = false };
                    }
                    var existingReading = await _meterReadingRepository.GetCurrentMeterReadingAsync(meterReading.roomId, meterReading.serviceId, meterReading.billingMonth, meterReading.billingYear);

                    if (existingReading != null)
                    {
                        return new Response<string> { Message = "Đã có số liệu đọc cho phòng dịch và dịch vụ trong tháng này", Succeeded = false };
                    }
                    await AddMeterReadingAsync(meterReading.roomId, meterReading.serviceId, meterReading.previousReading, meterReading.currentReading, meterReading.billingMonth, meterReading.billingYear);
                }
                return new Response<string> { Message = "Ghi số liệu thành công", Succeeded = true };
            }
            catch (Exception ex)
            {
                return new Response<string> { Message = ex.Message, Succeeded = false };
            }
        }

        public async Task<Response<List<ServiceCostReadingResponse>>> GetServiceCostReadingAsync(Guid hostelId, Guid roomId, int? billingMonth, int? billingYear)
        {
            try
            {
                // lấy ra tất cả các dịch vụ của trọ
                var serviceCosts = await _serviceCostService.GetAllServiceCostByHostel(hostelId);
                var serviceCostReadingResponses = new List<ServiceCostReadingResponse>();
                foreach (var serviceCost in serviceCosts.Data)
                {
                    if (serviceCost.ChargingMethod == ChargingMethod.PerUsageUnit)
                    {
                        MeterReading meterPreviousReading = null;
                        MeterReading meterCurrentReading = null;
                        if (billingMonth.HasValue && billingYear.HasValue)
                        {
                            meterPreviousReading = await _meterReadingRepository.GetPreviousMeterReadingAsync(roomId, serviceCost.ServiceId, billingMonth.Value, billingYear.Value);
                            meterCurrentReading = await _meterReadingRepository.GetCurrentMeterReadingAsync(roomId, serviceCost.ServiceId, billingMonth.Value, billingYear.Value);
                        }
                        else
                        {
                            meterPreviousReading = await _meterReadingRepository.GetPreviousMeterReadingAsync(roomId, serviceCost.ServiceId, DateTime.Now.Month, DateTime.Now.Year);
                            meterCurrentReading = await _meterReadingRepository.GetCurrentMeterReadingAsync(roomId, serviceCost.ServiceId, DateTime.Now.Month, DateTime.Now.Year);
                        }
                        var serviceCostReadingResponse = new ServiceCostReadingResponse
                        {
                            ServiceName = serviceCost.ServiceName,
                            UnitCost = serviceCost.UnitCost,
                            ChargingMethod = serviceCost.ChargingMethod,
                            Unit = serviceCost.Unit,
                            ServiceId = serviceCost.ServiceId,
                            PreviousReading = meterPreviousReading == null ? 0 : meterPreviousReading.Reading,
                            CurrentReading = meterCurrentReading == null ? 0 : meterCurrentReading.Reading,
                        };
                        serviceCostReadingResponses.Add(serviceCostReadingResponse);
                    }
                }

                return new Response<List<ServiceCostReadingResponse>>
                { Data = serviceCostReadingResponses, Succeeded = true };
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new Response<List<ServiceCostReadingResponse>> { Message = ex.Message, Succeeded = false });
            }
        }

        public async Task<PagedResponse<List<MeterReadingDto>>> GetPagedMeterReadingsAsync(int pageIndex, int pageSize, Guid hostelId, string? roomName = null, int? month = null, int? year = null)
        {
            try
            {
                var result = await _meterReadingRepository.GetPagedMeterReadingsAsync(pageIndex, pageSize, hostelId, roomName, month, year);

                var meterReadingDtos = _mapper.Map<List<MeterReadingDto>>(result.Data);

                var pagedResponse = PaginationHelper.CreatePagedResponse(
                    meterReadingDtos, pageIndex, pageSize, result.TotalRecords);

                return pagedResponse;
            }
            catch (Exception ex)
            {
                return new PagedResponse<List<MeterReadingDto>>
                {
                    Succeeded = false,
                    Message = "An error occurred while fetching meter readings.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }


        public async Task<Response<bool>> EditMeterReadingAsync(Guid id, EditMeterReadingDto dto)
        {
            try
            {
                var meterReading = await _meterReadingRepository.GetByIdAsync(id);
                if (meterReading == null)
                    return new Response<bool>("Không tìm thấy bản ghi số liệu.");
                var invoice = await _inVoiceRepository.GetInvoiceByRoomIdAndMonthYearAsync(meterReading.RoomId,
                    meterReading.BillingMonth, meterReading.BillingYear);
                if (invoice == null)
                {
                    meterReading.Reading = dto.Reading;
                    meterReading.LastModifiedOn = DateTime.Now;
                }
                if (invoice != null)
                {
                    if (invoice.IsPaid && !invoice.IsDeleted)
                    {
                        return new Response<bool>{Succeeded = false, Message = "Không thể sửa số liệu đã được thanh toán"};
                    }
                }

            await _meterReadingRepository.UpdateAsync(meterReading);

                return new Response<bool>(true, "Sửa bản ghi điện nước thành công.");
            }
            catch (Exception ex)
            {
                return new Response<bool>
                {
                    Succeeded = false,
                    Message = "An error occurred while updating the meter reading.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<Response<bool>> DeleteMeterReadingAsync(Guid id)
        {
            try
            {
                var meterReading = await _meterReadingRepository.GetByIdAsync(id);
                if (meterReading == null)
                    return new Response<bool>("Không tìm thấy số liệu để xóa.");
                var invoice = await _inVoiceRepository.GetInvoiceByRoomIdAndMonthYearAsync(meterReading.RoomId,
                    meterReading.BillingMonth, meterReading.BillingYear);
                if (invoice != null)
                {
                    if (invoice.IsPaid)
                    {
                        return new Response<bool>{Succeeded = false, Message = "Không thể xóa số liệu đã được thanh toán"};
                    }
                }
                await _meterReadingRepository.DeleteAsync(id);
                return new Response<bool>(true, "Xóa bản ghi điện nước thành công.");
            }
            catch (Exception ex)
            {
                return new Response<bool>
                {
                    Succeeded = false,
                    Message = "An error occurred while deleting the meter reading.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
