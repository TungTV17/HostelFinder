using AutoMapper;
using HostelFinder.Application.DTOs.RentalContract.Request;
using HostelFinder.Application.DTOs.RentalContract.Response;
using HostelFinder.Application.DTOs.Room.Responses;
using HostelFinder.Application.DTOs.Tenancies.Requests;
using HostelFinder.Application.DTOs.Tenancies.Responses;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;
        private readonly IS3Service _s3Service;
        private readonly IRoomTenancyRepository _roomTenancyRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IRentalContractRepository _rentalContractRepository;

        public TenantService(ITenantRepository tenantRepository,
            IMapper mapper,
            IS3Service s3Service,
            IRoomTenancyRepository roomTenancyRepository,
            IRentalContractRepository rentalContractRepository,
            IRoomRepository roomRepository)
        {
            _tenantRepository = tenantRepository;
            _mapper = mapper;
            _s3Service = s3Service;
            _roomTenancyRepository = roomTenancyRepository;
            _rentalContractRepository = rentalContractRepository;
            _roomRepository = roomRepository;
        }

        public async Task<TenantResponse> AddTenentServiceAsync(AddTenantDto request)
        {
            try
            {
                var checkIdentityCardNumber = await _tenantRepository.GetByIdentityCardNumber(request.IdentityCardNumber);
                if (checkIdentityCardNumber != null)
                {
                    throw new Exception($"Đã tồn tại cccd của {checkIdentityCardNumber.FullName}");
                }
                var tenent = _mapper.Map<Tenant>(request);
                // Kiểm tra nếu không có AvatarImage thì sử dụng URL mặc định
                if (request.AvatarImage != null)
                {
                    tenent.AvatarUrl = await _s3Service.UploadFileAsync(request.AvatarImage);
                }
                else
                {
                    tenent.AvatarUrl = "https://hostel-finder-images.s3.ap-southeast-1.amazonaws.com/Default-Avatar.png";
                }
                //upload image CCCD
                if(request.FrontImageImage != null)
                {
                    tenent.FrontImageUrl = await _s3Service.UploadFileAsync(request.FrontImageImage);
                }
                if(request.BackImageImage != null)
                {
                    tenent.BackImageUrl = await _s3Service.UploadFileAsync(request.BackImageImage);
                }
                tenent.CreatedOn = DateTime.Now;

                var tenentCreated = await _tenantRepository.AddAsync(tenent);

                //map to Dto 
                var tenentCreatedDto = _mapper.Map<TenantResponse>(tenentCreated);
                return tenentCreatedDto;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //lấy ra thông tin người thuê phòng tại phòng nào đó
        public async Task<List<InformationTenacyReponseDto>> GetInformationTenacyAsync(Guid roomId)
        {
            try
            {
                // lấy ra bản hợp đồng mới nhất
                var roomTenacy = await _roomTenancyRepository.GetRoomTenacyByIdAsync(roomId);

                //map to response 
                var informationTenacyRoomListDto = _mapper.Map<List<InformationTenacyReponseDto>>(roomTenacy);
                return informationTenacyRoomListDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Response<string>> AddRoommateAsync(AddRoommateDto request)
        {
            // Kiểm tra phòng tồn tại
            var room = await _roomRepository.GetByIdAsync(request.RoomId);
            if (room == null)
            {
                return new Response<string> { Message = "Phòng không tồn tại", Succeeded = false };
            }

            // Kiểm tra số lượng tenant hiện tại trong phòng
            var currentTenantsCount = await _roomTenancyRepository.CountCurrentTenantsAsync(request.RoomId);
            if (currentTenantsCount >= room.MaxRenters)
            {
                return new Response<string> { Message = "Phòng đã đầy, không thể thêm người thuê", Succeeded = false };
            }

            // Tạo tenant mới từ DTO
            var tenant = _mapper.Map<Tenant>(request);
            tenant.CreatedOn = DateTime.Now;

            // Kiểm tra nếu không có AvatarImage thì sử dụng URL mặc định
            if (request.AvatarImage != null)
            {
                tenant.AvatarUrl = await _s3Service.UploadFileAsync(request.AvatarImage);
            }
            else
            {
                tenant.AvatarUrl = "https://hostel-finder-images.s3.ap-southeast-1.amazonaws.com/Default-Avatar.png";
            }

            tenant.FrontImageUrl = await _s3Service.UploadFileAsync(request.FrontImageImage);
            tenant.BackImageUrl = await _s3Service.UploadFileAsync(request.BackImageImage);

            // Thêm tenant vào database
            var tenantCreated = await _tenantRepository.AddAsync(tenant);

            // Lấy thông tin hợp đồng RentalContract của phòng
            var rentalContract = await _rentalContractRepository.GetActiveRentalContractAsync(request.RoomId);

            if (rentalContract == null)
            {
                return new Response<string> { Message = "Không tìm thấy hợp đồng thuê cho phòng này", Succeeded = false };
            }

            // Sử dụng StartDate và EndDate từ hợp đồng để set MoveInDate và MoveOutDate
            DateTime? moveOutDate = rentalContract.EndDate;

            // Tạo bản ghi RoomTenancy để liên kết tenant và room
            var roomTenancy = new RoomTenancy
            {
                TenantId = tenantCreated.Id,
                RoomId = request.RoomId,
                MoveInDate = request.MoveInDate,
                MoveOutDate = moveOutDate, // MoveOutDate có thể là null nếu hợp đồng chưa kết thúc
                CreatedOn = DateTime.Now,
            };

            // Thêm bản ghi RoomTenancy
            await _roomTenancyRepository.AddAsync(roomTenancy);

            return new Response<string> { Message = "Thêm người thuê vào phòng thành công", Succeeded = true };
        }

        public async Task<PagedResponse<List<InformationTenanciesResponseDto>>> GetAllTenantsByHostelAsync(Guid hostelId, string? roomName, int pageNumber, int pageSize, string? status)
        {
            // Lấy danh sách tất cả RoomTenancy liên quan đến hostel
            var tenants = await _tenantRepository.GetTenantsByHostelAsync(hostelId, roomName, pageNumber, pageSize, status);

            // Trả về kết quả phân trang
            return tenants;
        }

        public async Task<Response<string>> MoveOutAsync(Guid tenantId, Guid roomId)
        {
            // Lấy RoomTenancy sớm nhất của phòng (dựa trên RoomId từ RentalContract)
            var roomTenancy = await _roomTenancyRepository.GetEarliestRoomTenancyByRoomIdAsync(roomId);

            // Kiểm tra nếu roomTenancy không tồn tại
            if (roomTenancy == null)
            {
                return new Response<string>
                {
                    Succeeded = false,
                    Message = "Không tìm thấy thông tin người thuê trong phòng này."
                };
            }

            // Kiểm tra TenantId nếu không khớp
            if (roomTenancy.TenantId != tenantId)
            {
                // Nếu TenantId không khớp, tìm RoomTenancy với TenantId yêu cầu và cập nhật MoveOutDate
                var tenantTenancy = await _roomTenancyRepository.GetRoomTenancyByTenantIdAsync(tenantId);

                if (tenantTenancy == null)
                {
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = "Không tìm thấy RoomTenancy cho tenant này."
                    };
                }

                if (tenantTenancy.MoveOutDate < DateTime.Now)
                {
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = $"{tenantTenancy.Tenant.FullName} đã rời phòng vào ngày {tenantTenancy.MoveOutDate}."
                    };
                }


                // Cập nhật MoveOutDate của tenant yêu cầu
                tenantTenancy.MoveOutDate = DateTime.Now;
                tenantTenancy.LastModifiedOn = DateTime.Now;
                tenantTenancy.IsDeleted = true;
                await _roomTenancyRepository.UpdateAsync(tenantTenancy);

                return new Response<string>
                {
                    Succeeded = true,
                    Message = $"{tenantTenancy.Tenant.FullName} đã rời phòng thành công."
                };
            }
            else
            {
                // Nếu TenantId khớp, xử lý các bước cập nhật hợp đồng, phòng, các tenant trong phòng
                var rentalContract = await _rentalContractRepository.GetRentalContractByRoomIdAsync(roomId);
                if (rentalContract == null)
                {
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = "Hiện tại không có hợp đồng nào cho phòng này."
                    };
                }

                if (rentalContract.EndDate < DateTime.Now)
                {
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = $"Hợp đồng cho phòng này đã kết thúc vào ngày {rentalContract.EndDate}."
                    };
                }

                // Cập nhật lại trạng thái hợp đồng: EndDate và LastModifiedOn
                rentalContract.EndDate = DateTime.Now;
                rentalContract.LastModifiedOn = DateTime.Now;
                rentalContract.IsDeleted = true;
                await _rentalContractRepository.UpdateAsync(rentalContract);

                // Cập nhật lại trạng thái của phòng
                var room = await _roomRepository.GetRoomByIdAsync(rentalContract.RoomId);
                if (room == null)
                {
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = "Phòng không tồn tại."
                    };
                }

                room.IsAvailable = true;
                room.LastModifiedOn = DateTime.Now;
                await _roomRepository.UpdateAsync(room);

                // Cập nhật lại trạng thái người thuê trong phòng
                var listTenancyInRoom = await _roomTenancyRepository.GetRoomTenacyByIdAsync(rentalContract.RoomId);
                foreach (var tenancy in listTenancyInRoom)
                {
                    tenancy.MoveOutDate = DateTime.Now;
                    tenancy.LastModifiedOn = DateTime.Now;
                    tenancy.IsDeleted = true;
                    await _roomTenancyRepository.UpdateAsync(tenancy);
                }

                return new Response<string>
                {
                    Succeeded = true,
                    Message = $"Đã trả phòng {room.RoomName} thành công."
                };
            }
        }

        public async Task<Response<string>> UpdateTenantAsync(UpdateTenantDto request)
        {
            try
            {
            
                var tenant = await _tenantRepository.GetByIdAsync(request.Id);
                if (tenant == null)
                {
                    return new Response<string> { Message = "Không tìm thấy thông tin người thuê", Succeeded = false };
                }
                var checkIdentityCardNumber = await _tenantRepository.GetByIdentityCardNumber(request.IdentityCardNumber);
                if (checkIdentityCardNumber != null && checkIdentityCardNumber.Id != request.Id)
                {
                    return new Response<string> { Message = $"Đã tồn tại CCCD của {checkIdentityCardNumber.FullName}", Succeeded = false };
                }
                tenant.FullName = request.FullName;
                tenant.AvatarUrl = request.AvatarImage != null ? await _s3Service.UploadFileAsync(request.AvatarImage) : tenant.AvatarUrl;
                tenant.Email = request.Email;
                tenant.Phone = request.Phone;
                tenant.IdentityCardNumber = request.IdentityCardNumber;
                tenant.FrontImageUrl = request.FrontImageImage != null ? await _s3Service.UploadFileAsync(request.FrontImageImage) : tenant.FrontImageUrl;
                tenant.BackImageUrl = request.BackImageImage != null ? await _s3Service.UploadFileAsync(request.BackImageImage) : tenant.BackImageUrl;
                tenant.Description = request.Description;
                tenant.TemporaryResidenceStatus = request.TemporaryResidenceStatus;
                
                //update tenant
                await _tenantRepository.UpdateAsync(tenant);
                return new Response<string> { Message = "Cập nhật thông tin người thuê thành công", Succeeded = true };
            }
            catch (Exception ex)
            {
                return new Response<string> { Message = ex.Message, Succeeded = false };
            }
        }

        public async Task<Response<TenantResponseDto>> GetTenantByIdAsync(Guid tenantId)
        {
            try
            {
                var tenant = await _tenantRepository.GetByIdAsync(tenantId);
                if (tenant == null)
                {
                    return new Response<TenantResponseDto> { Message = "Không tìm thấy thông tin người thuê", Succeeded = false };
                }
                var tenantDto = _mapper.Map<TenantResponseDto>(tenant);
                return new Response<TenantResponseDto> { Data = tenantDto, Succeeded = true };
            }
            catch (Exception ex)
            {
               return new Response<TenantResponseDto> { Message = ex.Message, Succeeded = false };
            }
        }
    }
}
