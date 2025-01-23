using AutoMapper;
using HostelFinder.Application.DTOs.Email;
using HostelFinder.Application.DTOs.Invoice.Responses;
using HostelFinder.Application.DTOs.InVoice.Requests;
using HostelFinder.Application.DTOs.InVoice.Responses;
using HostelFinder.Application.DTOs.Room.Responses;
using HostelFinder.Application.Helpers;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;
using HostelFinder.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HostelFinder.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInVoiceRepository _invoiceRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IMeterReadingRepository _meterReadingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceService> _logger;
        private readonly IServiceRepository _serviceRepository;
        private readonly IRoomTenancyRepository _roomTenancyRepository;
        private readonly IServiceCostRepository _serviceCostRepository;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public InvoiceService(IInVoiceRepository invoiceRepository,
            IRoomRepository roomRepository,
            IMeterReadingRepository meterReadingRepository,
            IMapper mapper,
            ILogger<InvoiceService> logger,
            IServiceRepository serviceRepository,
            IRoomTenancyRepository roomTenancyRepository,
            IServiceCostRepository serviceCostRepository,
            IEmailService emailService,
            IUserRepository userRepository)
        {
            _invoiceRepository = invoiceRepository;
            _roomRepository = roomRepository;
            _meterReadingRepository = meterReadingRepository;
            _mapper = mapper;
            _logger = logger;
            _serviceRepository = serviceRepository;
            _roomTenancyRepository = roomTenancyRepository;
            _serviceCostRepository = serviceCostRepository;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task<Response<List<InvoiceResponseDto>>> GetAllAsync()
        {
            var invoices = await _invoiceRepository.ListAllAsync();
            var result = _mapper.Map<List<InvoiceResponseDto>>(invoices);
            return new Response<List<InvoiceResponseDto>>(result);
        }

        public async Task<Response<InvoiceResponseDto>> GetByIdAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                return new Response<InvoiceResponseDto>("Invoice not found.");

            var result = _mapper.Map<InvoiceResponseDto>(invoice);
            return new Response<InvoiceResponseDto>(result);
        }

        public async Task<Response<InvoiceResponseDto>> GetDetailInvoiceByIdAsync(Guid id)
        {
            try
            {
                // lấy ra hóa đơn cuối cùng
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id); 
                if (invoice == null)
                {
                    return null;
                }
                var numberOfTenants = await _roomTenancyRepository.CountCurrentTenantsByRoomsInMonthAsync(invoice.RoomId, invoice.BillingMonth, invoice.BillingYear);
               

                InvoiceResponseDto invoiceResponseDto = new InvoiceResponseDto()
                {
                    Id = invoice.Id,
                    RoomId = invoice.RoomId,
                    RoomName = invoice.Room.RoomName,
                    TotalAmount = invoice.TotalAmount,
                    BillingMonth = invoice.BillingMonth,
                    BillingYear = invoice.BillingYear,
                    IsPaid = invoice.IsPaid,
                    FormOfTransfer = invoice.FormOfTransfer,
                    AmountPaid = invoice.AmountPaid,
                    InvoiceDetails = invoice.InvoiceDetails.Select(details => new InvoiceDetailResponseDto
                    {
                        ActualCost = details.ActualCost,
                        BillingDate = details.BillingDate,
                        CurrentReading = details.CurrentReading,
                        PreviousReading = details.PreviousReading,
                        InvoiceId = details.InvoiceId,
                        NumberOfCustomer = numberOfTenants,
                        ServiceName = details.Service?.ServiceName ??
                                      (details.IsRentRoom ? "Tiền thuê phòng" : "Không xác định"),
                        UnitCost = details.UnitCost,
                    }).ToList()
                };


                return new Response<InvoiceResponseDto>{Data = invoiceResponseDto, Succeeded = true};
            }
            catch (Exception ex)
            {

                return new Response<InvoiceResponseDto>() { Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<Response<string>> CollectMoneyInvoice(CollectMoneyInvoiceRequest request)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(request.InvoiceId);
                if (invoice == null)
                {
                    return new Response<string>() { Message = "Hóa đơn không tồn tại", Succeeded = false };
                }
                // kiểm tra xem hóa đơn đã thanh toán chưa
                if (invoice.IsPaid == true)
                {
                    return new Response<string>() { Message = "Hóa đơn đã đựợc thanh toán", Succeeded = false };
                }

                if (request.AmountPaid < 0)
                {
                    return new Response<string>() {Succeeded = false, Message = "Số tiền thanh toán không hợp lệ"};
                }

                if (request.AmountPaid == null)
                {
                    request.AmountPaid = invoice.TotalAmount;
                }
                invoice.IsPaid = true;
                invoice.AmountPaid = request.AmountPaid;
                invoice.LastModifiedOn = request.DateOfSubmit;
                invoice.FormOfTransfer = request.FormOfTransfer;
                
                await _invoiceRepository.UpdateAsync(invoice);
                return new Response<string>(){Message = $"Thu tiền hóa đơn tháng {invoice.BillingMonth}/{invoice.BillingYear} ở {invoice.Room.RoomName} thành công ", Succeeded = true};
            }
            catch (Exception ex)
            {
                return new Response<string>() { Message = ex.Message };
            }
        }

        public async Task<bool> CheckInvoiceNotPaidAsync(Guid roomId)
        {
            var invoices = await _invoiceRepository.GetInvoicesByRoomIdAsync(roomId);
            if(invoices.Count() == 0)
            {
                return false;
            }
            foreach (var invoice in invoices)
            {
                if (invoice.IsPaid == false)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> SendEmailInvoiceToTenantAsync(Guid landlordId,Guid invoiceId)
        {
            try
            {
                var invoiceDetails = await GetDetailInvoiceByIdAsync(invoiceId);
                if (invoiceDetails == null)
                {
                    return false;
                }
                // gửi email thông báo hóa đơn
                if (invoiceDetails.Data.IsPaid)
                {
                    var emailSubject = SendEmailInvoice.SUBJECT_INVOICE_SUCCESS;
                    var emailBody = SendEmailInvoice.BodyInvoiceSuccessEmail(invoiceDetails.Data);
                    // lấy ra email của người thuê trọ
                    var tenant = await _roomTenancyRepository.GetRoomTenancyRepresentativeAsync(invoiceDetails.Data.RoomId);
                    var email = tenant.Tenant.Email;
                    await _emailService.SendEmailAsync(email, emailSubject, emailBody);
                }
                else
                {
                    var emailSubject = SendEmailInvoice.SUBJECT_INVOICE;
                    var landlord = await _userRepository.GetByIdAsync(landlordId);
                    var emailBody = SendEmailInvoice.BodyInvoiceEmail(landlord,invoiceDetails.Data);
                    // lấy ra email của người thuê trọ
                    var tenant = await _roomTenancyRepository.GetRoomTenancyRepresentativeAsync(invoiceDetails.Data.RoomId);
                    var email = tenant.Tenant.Email;
                    await _emailService.SendEmailAsync(email, emailSubject, emailBody);
                }
                return true;

            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email invoice to tenant");
                throw new Exception(ex.Message);
            }
        }

        public async Task<Response<InvoiceResponseDto>> CreateAsync(AddInVoiceRequestDto invoiceDto)
        {
            var invoice = _mapper.Map<Invoice>(invoiceDto);
            invoice = await _invoiceRepository.AddAsync(invoice);

            var result = _mapper.Map<InvoiceResponseDto>(invoice);
            return new Response<InvoiceResponseDto>(result, "Invoice created successfully.");
        }

        public async Task<Response<InvoiceResponseDto>> UpdateAsync(Guid id, UpdateInvoiceRequestDto invoiceDto)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                return new Response<InvoiceResponseDto>("Invoice not found.");

            _mapper.Map(invoiceDto, invoice);
            invoice = await _invoiceRepository.UpdateAsync(invoice);

            var result = _mapper.Map<InvoiceResponseDto>(invoice);
            return new Response<InvoiceResponseDto>(result, "Invoice updated successfully.");
        }

        public async Task<Response<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id);
                if (invoice == null)
                    return new Response<bool>(false, "Không tìm thấy hóa đơn.");
                if(invoice.IsPaid)
                {
                    return new Response<bool>(false, "Không thể xóa hóa đơn đã thanh toán.");
                }
                await _invoiceRepository.DeleteAsync(id);
                return new Response<bool>(true, "Xóa hóa đơn thành công.");
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error occurred: {ex.Message}");
            }
        }

        public async Task<Response<InvoiceResponseDto>> GenerateMonthlyInvoicesAsync(Guid roomId, int billingMonth, int billingYear)
        {
            var transaction = await _invoiceRepository.BeginTransactionAsync();
            try
            {
                
                var room = await _roomRepository.GetRoomByIdAsync(roomId);
                if (room == null)
                {
                    return new Response<InvoiceResponseDto> { Message = "Phòng không tồn tại!", Succeeded = false };
                }
                if (room.IsAvailable)
                {
                    return new Response<InvoiceResponseDto> { Message = "Phòng đang trống không cần lập hóa đơn", Succeeded = false };
                }
                var existingInvoice = room.Invoices.FirstOrDefault(i => i.BillingMonth == billingMonth && i.BillingYear == billingYear && !i.IsDeleted);

                if (existingInvoice != null)
                {
                    return new Response<InvoiceResponseDto> { Message = "Đã có hóa đơn cho tháng này, nếu có sai sót trong quá trình nhập liệu bạn có thể chỉnh sửa", Succeeded = false };
                }

                //Tạo hóa đơn mới
                DateTime billingDate = new DateTime(billingYear, billingMonth, 1);
                var invoice = new Invoice
                {
                    RoomId = roomId,
                    BillingMonth = billingMonth,
                    BillingYear = billingYear,
                    TotalAmount = 0,
                    IsPaid = false,
                    CreatedOn = DateTime.Now,
                    InvoiceDetails = new List<InvoiceDetail>()
                };

                var serviceCosts = await _serviceCostRepository.GetServiceCostForDateByHostelAsync(room.HostelId, billingDate);
              

                foreach (var serviceCost in serviceCosts)
                {
                    var service = await _serviceRepository.GetServiceByIdAsync(serviceCost.ServiceId);

                    decimal detailTotalAmount = 0;
                    var invoiceDetail = new InvoiceDetail
                    {
                        InvoiceId = invoice.Id,
                        ServiceId = service.Id,
                        UnitCost = serviceCost.UnitCost,
                        ActualCost = 0,
                        NumberOfCustomer = await _roomTenancyRepository.CountCurrentTenantsByRoomsInMonthAsync(room.Id, billingMonth, billingYear),
                        BillingDate = DateTime.Now,
                        CreatedOn = DateTime.Now,
                        IsRentRoom = false,
                    };
                    // tạo hóa đơn chi tiết cho dịch vụ thu phí như điện, nước,...
                    switch (service.ChargingMethod)
                    {
                        case ChargingMethod.PerUsageUnit:
                            var previousReading =
                                await _meterReadingRepository.GetPreviousMeterReadingAsync(roomId, service.Id,
                                    billingMonth, billingYear);
                            var currentReading =
                                await _meterReadingRepository.GetCurrentMeterReadingAsync(roomId, service.Id,
                                    billingMonth, billingYear);

                            if (previousReading == null || currentReading == null)
                            {
                                return new Response<InvoiceResponseDto>
                                {
                                    Message = $"Thiếu số liệu đọc cho dịch vụ {service.ServiceName}.", Succeeded = false
                                };
                            }

                            if (currentReading.Reading < previousReading.Reading)
                            {
                                return new Response<InvoiceResponseDto>
                                {
                                    Message =
                                        $"Số đọc hiện tại không thể nhỏ hơn số đọc trước cho dịch vụ {service.ServiceName}.",
                                    Succeeded = false
                                };
                            }

                            invoiceDetail.PreviousReading = previousReading.Reading;
                            invoiceDetail.CurrentReading = currentReading.Reading;

                            var usage = invoiceDetail.CurrentReading - invoiceDetail.PreviousReading;
                            detailTotalAmount = invoiceDetail.UnitCost * usage;

                            invoiceDetail.ActualCost = detailTotalAmount;
                            invoiceDetail.NumberOfCustomer = null; // Not applicable
                            break;
                        case ChargingMethod.PerPerson:
                            var numberOfTenants = await _roomTenancyRepository.CountCurrentTenantsByRoomsInMonthAsync(room.Id, billingMonth,billingYear);
                            invoiceDetail.NumberOfCustomer = numberOfTenants;
                            detailTotalAmount = invoiceDetail.UnitCost * numberOfTenants;

                            invoiceDetail.ActualCost = detailTotalAmount;
                            invoiceDetail.CurrentReading = 0;
                            invoiceDetail.PreviousReading = 0;
                            break;

                        case ChargingMethod.FlatFee:
                            detailTotalAmount = invoiceDetail.UnitCost;
                            invoiceDetail.ActualCost = detailTotalAmount;
                            invoiceDetail.NumberOfCustomer = 0;
                            invoiceDetail.CurrentReading = 0;
                            invoiceDetail.PreviousReading = 0;
                            break;

                        case ChargingMethod.Free:
                            detailTotalAmount = 0;
                            invoiceDetail.ActualCost = detailTotalAmount;
                            invoiceDetail.NumberOfCustomer = 0;
                            invoiceDetail.CurrentReading = 0;
                            invoiceDetail.PreviousReading = 0;
                            break;

                        default:
                            return new Response<InvoiceResponseDto>
                            {
                                Message = $"Phương thức tính phí không xác định cho dịch vụ {service.ServiceName}.",
                                Succeeded = false
                            };
                    }
                    
                    invoice.InvoiceDetails.Add(invoiceDetail);
                    invoice.TotalAmount += detailTotalAmount;
                }


                //tạo hóa đơn tiền phòng
                var rentInvoiceDetail = new InvoiceDetail
                {
                    InvoiceId = invoice.Id,
                    Service = null,
                    UnitCost = room.MonthlyRentCost,
                    ActualCost = room.MonthlyRentCost,
                    NumberOfCustomer = room.MaxRenters,
                    BillingDate = DateTime.Now,
                    IsRentRoom = true,
                    CreatedOn = DateTime.Now,
                    CurrentReading = 0,
                    PreviousReading = 0,
                };
                invoice.InvoiceDetails.Add(rentInvoiceDetail);
                invoice.TotalAmount += rentInvoiceDetail.ActualCost;
                var invoiceCreated = await _invoiceRepository.AddAsync(invoice);

                await transaction.CommitAsync();

                //map to Dtos
                var invoiceCreatedDto = new InvoiceResponseDto
                {
                    Id = invoice.Id,
                    RoomName = room.RoomName ?? string.Empty,
                    BillingMonth = invoiceCreated.BillingMonth,
                    BillingYear = invoiceCreated.BillingYear,
                    IsPaid = invoiceCreated.IsPaid,
                    TotalAmount = invoiceCreated.TotalAmount,
                    InvoiceDetails = invoiceCreated.InvoiceDetails.Select(details => new InvoiceDetailResponseDto
                    {
                        ActualCost = details.ActualCost,
                        BillingDate = details.BillingDate,
                        CurrentReading = details.CurrentReading,
                        PreviousReading = details.PreviousReading,
                        InvoiceId = details.InvoiceId,
                        NumberOfCustomer = details.NumberOfCustomer,
                        ServiceName = details.Service?.ServiceName ?? (details.IsRentRoom ? "Tiền thuê phòng" : "Không xác định"),
                        UnitCost = details.UnitCost,
                    }).ToList()

                };
                return new Response<InvoiceResponseDto> { Data = invoiceCreatedDto, Message = $"Tạo hóa đơn thành công cho phòng {room.RoomName} vào cho tháng {billingMonth}-{billingYear}", Succeeded = true };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Xảy ra trong quá tình tạo hóa đơn phòng");
                return new Response<InvoiceResponseDto> { Message = "Xảy ra lỗi trong quá trình tạo hóa đơn" };
            }
        }

        public async Task<RoomInvoiceHistoryDetailsResponseDto?> GetInvoiceDetailInRoomLastestAsyc(Guid roomId)
        {
            try
            {
                // lấy ra hóa đơn cuối cùng
                var invoice = await _invoiceRepository.GetLastInvoiceByIdAsync(roomId);
                var numberOfTenants = await _roomTenancyRepository.CountCurrentTenantsAsync(roomId);
                if (invoice == null)
                {
                    return null;
                }

                RoomInvoiceHistoryDetailsResponseDto invoiceResponseDto = new RoomInvoiceHistoryDetailsResponseDto
                {
                    Id = invoice.Id,
                    TotalAmount = invoice.TotalAmount,
                    BillingMonth = invoice.BillingMonth,
                    BillingYear = invoice.BillingYear,
                    IsPaid = invoice.IsPaid,
                    InvoiceDetails = invoice.InvoiceDetails.Select(details => new InvoiceDetailResponseDto
                    {
                        ActualCost = details.ActualCost,
                        BillingDate = details.BillingDate,
                        CurrentReading = details.CurrentReading,
                        PreviousReading = details.PreviousReading,
                        InvoiceId = details.InvoiceId,
                        NumberOfCustomer = numberOfTenants,
                        ServiceName = details.Service?.ServiceName ?? (details.IsRentRoom ? "Tiền thuê phòng" : "Không xác định"),
                        UnitCost = details.UnitCost,
                    }).ToList()
                };
                

                return invoiceResponseDto;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Response<bool>> CheckInvoiceExistAsync(Guid roomId, int billingMonth, int billingYear)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null)
            {
                return new Response<bool> { Succeeded = false, Message = "Phòng không tồn tại" };
            }
            var invoice = room.Invoices.FirstOrDefault(i => i.BillingMonth == billingMonth && i.BillingYear == billingYear && !i.IsDeleted);
            if (invoice != null)
            {
                return new Response<bool>{Succeeded = true, Message = $"Đã có hóa đơn cho tháng {billingMonth}/{billingYear}"};
            }
            return new Response<bool>{Succeeded = false, Message = $"Chưa có hóa đơn cho tháng {billingMonth}/{billingYear}"};
        }

        public async Task<PagedResponse<List<ListInvoiceResponseDto>>> GetAllInvoicesByHostelIdAsync(Guid hostelId, string? searchPhrase, int? pageNumber, int? pageSize, string? sortBy,
            SortDirection sortDirection)
        {
            try
            {
                var listInvoices = await _invoiceRepository.GetAllMatchingInvoiceAysnc(hostelId, searchPhrase,pageNumber ?? 1, pageSize ?? 10, sortBy, sortDirection);
                var result = _mapper.Map<List<ListInvoiceResponseDto>>(listInvoices.invoices);
                var pagedResponse = PaginationHelper.CreatePagedResponse(result, pageNumber ?? 1, pageSize ?? 10, listInvoices.totalRecord);
                return pagedResponse;
            }
            catch (Exception ex)
            {
                return new PagedResponse<List<ListInvoiceResponseDto>> { Message = ex.Message };
            }
        }
    }
}
