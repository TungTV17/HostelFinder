using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using HostelFinder.Application.DTOs.RentalContract.Request;
using HostelFinder.Application.DTOs.RentalContract.Response;
using HostelFinder.Application.DTOs.Room.Responses;
using HostelFinder.Application.Helpers;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;
using Irony.Parsing.Construction;

namespace HostelFinder.Application.Services
{
    public class RentalContractService : IRentalContractService
    {
        private readonly IRentalContractRepository _rentalContractRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomTenancyRepository _roomTenancyRepository;
        private readonly ITenantService _tenantService;
        private readonly IMapper _mapper;
        private readonly IMeterReadingService _meterReadingService;
        private readonly IMeterReadingRepository _meterReadingRepository;
        public RentalContractService(
            IRentalContractRepository rentalContractRepository,
            IRoomRepository roomRepository,
            IRoomTenancyRepository roomTenancyRepository,
            ITenantService tenantService,
            IMapper mapper,
            IMeterReadingService meterReadingService,
            IMeterReadingRepository meterReadingRepository)
        {
            _rentalContractRepository = rentalContractRepository;
            _roomRepository = roomRepository;
            _roomTenancyRepository = roomTenancyRepository;
            _tenantService = tenantService;
            _mapper = mapper;
            _meterReadingService = meterReadingService;
            _meterReadingRepository = meterReadingRepository;
        }
        public async Task<Response<string>> CreateRentalContractAsync(AddRentalContractDto request)
        {
            try
            {
                // kiểm tra xem hợp đồng đã hết hạn chưa ?
                var checkExpiredContract = await _rentalContractRepository.CheckExpiredContractAsync(request.RoomId, request.StartDate, request.EndDate);
                // Nếu khác null là thì không nằm trong thời gian hợp đồng
                if (checkExpiredContract != null)
                {
                    return new Response<string> { Succeeded = false, Message = $"Hiện tại đã có hợp đồng tồn tại trong khoảng thời gian {checkExpiredContract.StartDate.ToString("dd/MM/yyyy")} - {checkExpiredContract.EndDate.Value.ToString("dd/MM/yyyy")}." +
                        $" Hoặc bạn có thể cập nhập lại thời hạn hợp đồng " };
                }

                // Kiểm tra số người thuê trọ hiện tại trong phòng
                var room = await _roomRepository.GetRoomByIdAsync(request.RoomId);
                if (room.IsAvailable = false)
                {
                    return new Response<string>() { Succeeded = false, Message = "Hiện tại phòng chưa sẵn sàng để cho thuê" };
                }
                var currentTenantsCount = await _roomTenancyRepository.CountCurrentTenantsAsync(room.Id);
                if (currentTenantsCount >= room.MaxRenters)
                {
                    return new Response<string> { Succeeded = false, Message = "Phòng hiện tại đã đạt tối đa số lượng người thuê" };
                }

              

                // tạo người thuê phòng
                
                var tenantCreated = await _tenantService.AddTenentServiceAsync(request.AddTenantDto);

                // tạo hợp đồng
                var rentalContract = new RentalContract
                {
                    TenantId = tenantCreated.Id,
                    RoomId = room.Id,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    MonthlyRent = request.MonthlyRent,
                    DepositAmount = request.DepositAmount,
                    PaymentCycleDays = request.PaymentCycleDays,
                    ContractTerms = request.ContractTerms,
                    CreatedOn = DateTime.Now,
                };

                


                // tạo mới roomTenacy cho người thuê chính
                var roomTenacy = new RoomTenancy
                {
                    TenantId = tenantCreated.Id,
                    RoomId = room.Id,
                    MoveInDate = rentalContract.StartDate,
                    MoveOutDate = rentalContract.EndDate,
                    CreatedOn = DateTime.Now
                };

                await _roomTenancyRepository.AddAsync(roomTenacy);


                // kiểm tra số lượng trong phòng
                if(currentTenantsCount + 1 >= 1 && rentalContract.StartDate <= DateTime.Now.Date &&(rentalContract ==null || rentalContract.EndDate >= DateTime.Now.Date))
                {
                    room.IsAvailable = false;
                   await _roomRepository.UpdateAsync(room);
                }

                await _rentalContractRepository.AddAsync(rentalContract);

                return new Response<string> { Succeeded = true, Message = $"Tạo hợp đồng cho thuê thành công cho phòng {room.RoomName} với người thuê : {tenantCreated.FullName}" };
            }
            catch (Exception ex)
            {
                return new Response<string> { Succeeded = false, Errors = new List<string> { ex.Message}, Message = ex.Message };
            }
        }

        public async Task<RoomContractHistoryResponseDto> GetRoomContractHistoryLasest(Guid roomId)
        {
            try
            {
                // lấy ra thông tin của hợp đồng theo room
                var getRoomContract = await _rentalContractRepository.GetRoomRentalContrctByRoom(roomId);
                if(getRoomContract == null)
                {
                    return null;
                }
             
                var roomrentalContractResponseDto = _mapper.Map<RoomContractHistoryResponseDto>(getRoomContract);
                return roomrentalContractResponseDto;
                    
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Response<string>> TerminationOfContract(Guid rentalContractId)
        {
            try
            {
                var rentalContract = await _rentalContractRepository.GetByIdAsync(rentalContractId);
                if (rentalContract == null)
                {
                    return new Response<string>() { Succeeded = false, Message = "Hiện tại không có hợp đồng nào" };
                }
                if (rentalContract.EndDate < DateTime.Now.Date)
                {
                    return new Response<string>()
                    {
                        Succeeded = false,
                        Message = $"Hợp đồng đã chấm dứt vào ngày {rentalContract.EndDate.Value.ToString("dd/MM/yyyy")}"
                    };
                }
                // Cập nhật lại trạng thái hợp đồng
                rentalContract.EndDate = DateTime.Now;
                rentalContract.LastModifiedOn = DateTime.Now;
                await _rentalContractRepository.UpdateAsync(rentalContract);
                
                //cập nhật lại trạng thái của phòng
                var room = await _roomRepository.GetRoomByIdAsync(rentalContract.RoomId);
                room.IsAvailable = true;
                room.LastModifiedOn = DateTime.Now;
                await _roomRepository.UpdateAsync(room);
                
                // cập nhật lại trạng thái người thuê phòng theo phòng
                var listTenacyInRoom = await _roomTenancyRepository.GetRoomTenacyByIdAsync(rentalContract.RoomId);
                foreach (var tenancy in listTenacyInRoom)
                {
                    tenancy.MoveOutDate = DateTime.Now;
                    tenancy.LastModifiedOn = DateTime.Now;
                    await _roomTenancyRepository.UpdateAsync(tenancy);
                }

                return new Response<string>() { Succeeded = true, Message = $"Chấm dứt hợp đồng với phòng trọ {room.RoomName} vào ngày {DateTime.Now.ToString("dd/MM/yyyy")}"};
            }
            catch (Exception ex)
            {
                return new Response<string> { Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<Response<string>> ContractExtension(Guid rentalContractId, DateTime newEndDate)
        {
            try
            {
                var contract = await _rentalContractRepository.GetByIdAsync(rentalContractId);
                if (contract == null)
                {
                    return new Response<string>() { Succeeded = false, Message = "Không tìm thấy hợp đồng" };
                }

                if (newEndDate < DateTime.Now.Date)
                {
                    return new Response<string>() { Succeeded = false, Message = "Ngày gia hạn hợp đồng không hợp lệ" };
                }
                if (newEndDate <= contract.EndDate)
                {
                    return new Response<string>() { Succeeded = false, Message = "Ngày gia hạn phải lớn hơn ngày kết thúc hợp đồng" };
                }
                contract.EndDate = newEndDate;
                contract.LastModifiedOn = DateTime.Now;
                await _rentalContractRepository.UpdateAsync(contract);

                var roomTenacies =
                    await _roomTenancyRepository.GetTenacyCurrentlyByRoom(contract.RoomId, contract.StartDate,
                        contract.EndDate);
                foreach (var roomTenacy in roomTenacies)
                {
                    roomTenacy.MoveOutDate = newEndDate;
                    roomTenacy.LastModifiedOn = DateTime.Now;
                    await _roomTenancyRepository.UpdateAsync(roomTenacy);
                }
                return new Response<string>() { Succeeded = true, Message = $"Gia hạn hợp đồng thành công đến ngày {newEndDate.ToString("dd/MM/yyyy")}" };
                
                
            }catch(Exception ex)
            {
                return new Response<string>() { Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<PagedResponse<List<RentalContractResponseDto>>> GetRentalContractsByHostelIdAsync(Guid hostelId, string? searchPhrase,string? statusFilter, int? pageNumber, int? pageSize,
            string? sortBy, SortDirection sortDirection)
        {
            try
            {
                var listRentalContracts = await _rentalContractRepository.GetAllMatchingRentalContractAysnc(hostelId, searchPhrase,statusFilter, pageNumber ?? 1, pageSize ?? 10, sortBy, sortDirection);
                var result = _mapper.Map<List<RentalContractResponseDto>>(listRentalContracts.rentalContracts);
                foreach (var rentalContract in result)
                {
                    rentalContract.Status = GetContractStatus(rentalContract.StartDate, rentalContract.EndDate);
                }
                var pagedResponse = PaginationHelper.CreatePagedResponse(result, pageNumber ?? 1, pageSize ?? 10, listRentalContracts.totalRecord);
                return pagedResponse;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckContractExistAsync(Guid roomId)
        {
            var checkContract = await _rentalContractRepository.GetRoomRentalContrctByRoom(roomId);
            if (checkContract == null)
            {
                return false;
            }
            return true;
        }

        private string GetContractStatus(DateTime startDate, DateTime? endDate)
        {
            var currentDate = DateTime.Now.Date;
            if(endDate.HasValue && endDate.Value.Date < currentDate)
            {
                return "Đã kết thúc";
            }
            //hợp đồng sắp hết hạn sau 7 ngày
            if(endDate.HasValue && endDate.Value.AddMonths(-1) <= currentDate && endDate.Value.Date >= currentDate)
            {
                return "Sắp kết thúc";
            }

            if (startDate.Date <= currentDate && (!endDate.HasValue || endDate.Value.Date > currentDate))
            {
                return "Trong thời hạn";
            }

            return "Chưa bắt đầu";
        }
    }
}
