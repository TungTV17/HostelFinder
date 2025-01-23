using AutoMapper;
using HostelFinder.Application.DTOs.Room;
using HostelFinder.Application.DTOs.Room.Requests;
using HostelFinder.Application.DTOs.Room.Responses;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace HostelFinder.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomAmentityRepository _roomAmentityRepository;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;
        private readonly IImageRepository _imageRepository;
        private readonly ITenantService _tenantService;
        private readonly IRoomTenancyRepository _roomTenancyRepository;
        private readonly IInvoiceService _invoiceService;
        private readonly IRentalContractService _rentalContractService;
        public RoomService(IRoomRepository roomRepository, 
            IMapper mapper, 
            IRoomAmentityRepository roomAmentityRepository
            , IS3Service s3Service,
            IImageRepository imageRepository,
            ITenantService tenantService,
            IRoomTenancyRepository roomTenancyRepository,
            IInvoiceService invoiceService,
            IRentalContractService rentalContractService)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
            _roomAmentityRepository = roomAmentityRepository;
            _s3Service = s3Service;
            _imageRepository = imageRepository;
            _tenantService = tenantService;
            _roomTenancyRepository = roomTenancyRepository;
            _invoiceService = invoiceService;
            _rentalContractService = rentalContractService;
        }
        /// <summary>
        /// Lấy ra tất cả các phòng
        /// </summary>
        /// <returns></returns>
        public async Task<Response<List<RoomResponseDto>>> GetAllAsync()
        {
            var rooms = await _roomRepository.ListAllWithDetailsAsync();
            var result = _mapper.Map<List<RoomResponseDto>>(rooms);
            return new Response<List<RoomResponseDto>>(result);
        }

        /// <summary>
        /// Lấy phòng theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<RoomByIdDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var room = await _roomRepository.GetRoomByIdAsync(id);
                var result = _mapper.Map<RoomByIdDto>(room);
                result.ImageRoom = await _imageRepository.GetAllUrlRoomPicture(room.Id);
                return new Response<RoomByIdDto>(result);
            }
            catch (Exception ex)
            {
                return new Response<RoomByIdDto>() { Succeeded = false, Message = ex.Message };
            }
        }
        /// <summary>
        /// Tạo phòng theo từng trọ, chung cư
        /// </summary>
        /// <param name="roomDto"></param>
        /// <param name="roomImages"></param>
        /// <returns></returns>
        public async Task<Response<RoomResponseDto>> CreateRoomAsync(AddRoomRequestDto roomDto, List<IFormFile> roomImages)
        {
            bool roomExists = await _roomRepository.RoomExistsAsync(roomDto.RoomName, roomDto.HostelId);
            if (roomExists)
            {
                return new Response<RoomResponseDto>("Tên phòng đã tồn tại trong trọ.");
            }
            //map to Domain 
            var room = _mapper.Map<Room>(roomDto);
            room.IsAvailable = true;
            room.CreatedBy = room.HostelId.ToString();
            room.CreatedOn = DateTime.Now;
            room.IsDeleted = false;

            //add to db
            var roomAdded = await _roomRepository.AddAsync(room);

            //upload image to AWS and collect return Image response

            var imageUrls = new List<string>();

            if (roomImages != null && roomImages.Count > 0)
            {
                foreach (var image in roomImages)
                {
                    var uploadToAWS3 = await _s3Service.UploadFileAsync(image);
                    var imageUrl = uploadToAWS3;
                    imageUrls.Add(imageUrl);
                }
            }

            //add image to db
            foreach(var imageUrl in imageUrls)
            {
                await _imageRepository.AddAsync(new Image
                {
                    RoomId = roomAdded.Id,
                    Url = imageUrl,
                    CreatedOn = DateTime.Now,
                    CreatedBy = roomAdded.HostelId.ToString(),
                    HostelId = roomAdded.HostelId
                });
            }


            List<Guid> amentityIds = roomDto.AmenityId.ToList();

            //add amentityRoom to db
            foreach(var amentityId in amentityIds)
            {
                await _roomAmentityRepository.AddAsync(new RoomAmenities
                {
                    AmenityId = amentityId,
                    RoomId = roomAdded.Id,
                    CreatedOn = DateTime.Now,
                    CreatedBy = roomAdded.HostelId.ToString(),
                    IsDeleted = false
                });
            }



            return new Response<RoomResponseDto>
            {
                 Succeeded = true,
                 Message = "Thêm phòng trọ thành công"
            };
        }
        /// <summary>
        /// Update phòng trọ theo từng tró
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roomDto"></param>
        /// <returns></returns>
        public async Task<Response<RoomResponseDto>> UpdateAsync(Guid id, UpdateRoomRequestDto roomDto, List<IFormFile> roomImages)
        {
            try
            {
                var room = await _roomRepository.GetRoomByIdAsync(id);
                if (room == null)
                    return new Response<RoomResponseDto>("Phòng không tồn tại");
                // bool roomExists = await _roomRepository.RoomExistsAsync(roomDto.RoomName, roomDto.HostelId);
                // if (roomExists)
                // {
                //     return new Response<RoomResponseDto>("Tên phòng đã tồn tại trong trọ.");
                // }

                room.RoomName = roomDto.RoomName;
                room.Floor = roomDto.Floor;
                room.MaxRenters = roomDto.MaxRenters;
                room.Deposit = roomDto.Deposit;
                room.MonthlyRentCost = roomDto.MonthlyRentCost;
                room.Size = roomDto.Size;
                room.RoomType = roomDto.RoomType;
                // kiểm tra xem phòng có người thuê không
                if (roomDto.IsAvailable == true)
                {
                    // kiểm tra xem có hợp đồng không thì mới cho chuyển thành true 
                    var checkContract = await _rentalContractService.CheckContractExistAsync(room.Id);
                    if (checkContract)
                    {
                        return new Response<RoomResponseDto>("Phòng đang có hợp đồng không thể chuyển trạng thái trống");
                    }
                }
                room.IsAvailable = roomDto.IsAvailable;
                var updatedRoom = await _roomRepository.UpdateAsync(room);
                // Xử lý hình ảnh phòng
                var imageUrls = new List<string>();
                if (roomImages != null && roomImages.Count > 0)
                {
                    // Xóa hình ảnh cũ của phòng
                    await _imageRepository.DeleteByRoomId(updatedRoom.Id);

                    // Tải lên hình ảnh mới lên AWS và lưu lại URL
                    foreach (var image in roomImages)
                    {
                        var uploadToAWS3 = await _s3Service.UploadFileAsync(image);
                        var imageUrl = uploadToAWS3;
                        imageUrls.Add(imageUrl);
                    }

                    // Thêm hình ảnh mới vào DB
                    foreach (var imageUrl in imageUrls)
                    {
                        await _imageRepository.AddAsync(new Image
                        {
                            RoomId = updatedRoom.Id,
                            Url = imageUrl,
                            CreatedOn = DateTime.Now,
                            CreatedBy = updatedRoom.HostelId.ToString(),
                            HostelId = updatedRoom.HostelId
                        });
                    }
                }

                // Cập nhật tiện ích phòng
                List<Guid> amentityIds = roomDto.AmenityId.ToList();
                // Xóa các tiện ích cũ của phòng
                await _roomAmentityRepository.DeleteByRoomIdAsync(room.Id);
                // var roomAmenitiesList = amentityIds.Select(amentityId => new RoomAmenities
                // {
                //     AmenityId = amentityId,
                //     RoomId = updatedRoom.Id,
                //     LastModifiedOn = DateTime.Now,
                //     CreatedBy = updatedRoom.HostelId.ToString(),
                //     IsDeleted = false
                // }).ToList();
                var roomAmenitiesList = new List<RoomAmenities>();
                foreach (var amentityId in amentityIds)
                {
                    roomAmenitiesList.Add(new RoomAmenities()
                    {
                        AmenityId = amentityId,
                        RoomId = updatedRoom.Id,
                        LastModifiedOn = DateTime.Now,
                        CreatedBy = updatedRoom.HostelId.ToString(),
                        IsDeleted = false
                    });
                }

                // Thêm tiện ích vào DB
                await _roomAmentityRepository.AddRangeAsync(roomAmenitiesList);  
                var roomResponseDto = _mapper.Map<RoomResponseDto>(updatedRoom);
                
                return new Response<RoomResponseDto>(roomResponseDto, "Room updated successfully.");
            }
            catch (Exception ex)
            {
                return new Response<RoomResponseDto>() { Succeeded = false, Message = ex.Message};
            }
        }

        /// <summary>
        /// Xóa phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var room = await _roomRepository.GetRoomByIdAsync(id);
                if (room.IsAvailable == false)
                {
                    return new Response<bool>() { Succeeded = false, Message = "Phòng đang có người thuê không thể xóa." };
                }

                await _roomRepository.DeleteAsync(id);
                return new Response<bool>(true, "Xóa phòng thành công.");
            }
            catch (Exception ex)
            {
                return new Response<bool>() { Succeeded = false, Message = ex.Message };
            }
        }
        /// <summary>
        /// Lấy phòng theo id của trọ, theo tầng
        /// </summary>
        /// <param name="hostelId"></param>
        /// <param name="floor"></param>
        /// <returns></returns>
        public async Task<Response<List<RoomResponseDto>>> GetRoomsByHostelIdAsync(Guid hostelId, int? floor)
        {
            var rooms = await _roomRepository.GetRoomsByHostelIdAsync(hostelId, floor);

            if (rooms == null || !rooms.Any())
                return new Response<List<RoomResponseDto>> { Succeeded = true, Message = "Hiện tại chưa có phòng trọ nào" };


            var result = _mapper.Map<List<RoomResponseDto>>(rooms);


            foreach (var room in result)
            {
                var imageRoom = await _imageRepository.GetImageUrlByRoomId(room.Id);
                room.ImageRoom = imageRoom?.Url ?? "https://hostel-finder-images.s3.amazonaws.com/download%20(3)-9443562d-b4cf-41b5-b053-807457b0047c.jpg";
            }
            return new Response<List<RoomResponseDto>>(result);
        }

        public Task<Response<List<RoomServiceDto>>> GetServicesByRoom(Guid roomId)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Lấy ra tất cả thông tin phòng trọ bao gồm thông tin phòng, thông tin người thuê, lịch sử hóa đơn, dịch vụ phòng đang sử dụng
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public async Task<Response<GetAllInformationRoomResponseDto>> GetInformationDetailRoom(Guid roomId)
        {
            try
            {
                GetAllInformationRoomResponseDto getAllInformationRoomResponseDto = new GetAllInformationRoomResponseDto();
                //Lấy ra thông tin phòng chi tiết của phòng đó
                var roomDomain = await _roomRepository.GetByIdAsync(roomId);

                //map vào dto thông tin cần thiết
                var roomInfoDetailDto = _mapper.Map<RoomInfoDetailResponseDto>(roomDomain);

                roomInfoDetailDto.NumberOfCustomer = await _roomTenancyRepository.CountCurrentTenantsAsync(roomId);

                // thêm vào list 
                getAllInformationRoomResponseDto.RoomInfoDetail = roomInfoDetailDto;

                // lấy ra thông tin những người đang thuê ở trọ hiện tại
                var roomTenacyList = await _tenantService.GetInformationTenacyAsync(roomId);
                
                getAllInformationRoomResponseDto.InfomationTenacy = roomTenacyList;

                //lấy ra thông tin ảnh của phòng 
                getAllInformationRoomResponseDto.PictureRoom = await _imageRepository.GetAllUrlRoomPicture(roomId);

                // lấy ra hóa đơn thanh toán tháng trước
                getAllInformationRoomResponseDto.InvoiceDetailInRoom = await _invoiceService.GetInvoiceDetailInRoomLastestAsyc(roomId);

                // lấy ra thông tin hợp đồng hiện tại
                getAllInformationRoomResponseDto.ContractDetailInRoom = await _rentalContractService.GetRoomContractHistoryLasest(roomId);

                return new Response<GetAllInformationRoomResponseDto> { Data = getAllInformationRoomResponseDto, Succeeded = true};
            }
            catch (Exception ex)
            {
                return new Response<GetAllInformationRoomResponseDto> { Succeeded = false, Message = ex.Message };   
            }
        }

        public async Task<Response<EditRoomDtoResponse>> GetRoomWithAmentitesAndImageAsync(Guid roomId)
        {
            try
            {
                var room = await _roomRepository.GetRoomByIdAsync(roomId);
                var result = _mapper.Map<EditRoomDtoResponse>(room);
                result.ImageRoom = await _imageRepository.GetAllUrlRoomPicture(room.Id);
                var getAmentitesByRoom = await _roomAmentityRepository.GetAmenitiesByRoomIdAsync(room.Id);
                result.AmenityIds = getAmentitesByRoom.Select(x => x.AmenityId).ToList();
                return new Response<EditRoomDtoResponse>(){Succeeded = true, Data = result};
                
            }
            catch (Exception ex)
            {
                return new Response<EditRoomDtoResponse>() { Succeeded = false, Message = ex.Message };
            }
        }


        public async Task<Response<bool>> CheckDeleteRoom(Guid roomId)
        {
            try
            {
                var room = await _roomRepository.GetRoomByIdAsync(roomId);
                // kiểm tra xem phòng có người thuê không
                var countCurrentTenants = await _roomTenancyRepository.CountCurrentTenantsAsync(roomId);
                if(countCurrentTenants > 0)
                {
                    return new Response<bool>() { Succeeded = true, Message = "Phòng hiện tại đang có người thuê.", Data = false};
                }
                // kiểm tra xem phòng có hóa đơn chưa thanh toán không
                var checkInvoice = await _invoiceService.CheckInvoiceNotPaidAsync(roomId);
                if(checkInvoice)
                {
                    return new Response<bool>() { Succeeded = true, Message = "Phòng đang có hóa đơn chưa thanh toán, vui lòng kiểm tra lại", Data = false};
                }
                // kiểm tra xem phòng có hợp đồng không
                var checkContract = await _rentalContractService.CheckContractExistAsync(roomId);
                if(checkContract )
                {
                    return new Response<bool>() { Succeeded = true, Message = "Phòng đang có hợp đồng không thể xóa", Data = false};
                }
                // xóa phòng
                return new Response<bool>() { Succeeded = true, Message = "Có thể xóa phòng", Data = true};
            }
            catch (Exception ex)
            {
                return new Response<bool>() { Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<Response<List<SelectRoomResponse>>> GetSelectRoomByHostelAsync(Guid hostelId)
        {
            try
            {
                var rooms = await _roomRepository.GetRoomsByHostelAsync(hostelId);
                var result = _mapper.Map<List<SelectRoomResponse>>(rooms);
                return new Response<List<SelectRoomResponse>>() { Succeeded = true, Data = result };
            }
            catch (Exception ex)
            {
                return new Response<List<SelectRoomResponse>>() { Succeeded = false, Message = ex.Message };
            }
        }
    }
}
