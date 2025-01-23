using AutoMapper;
using HostelFinder.Application.DTOs.Hostel.Requests;
using HostelFinder.Application.DTOs.Hostel.Responses;
using HostelFinder.Application.Helpers;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using Microsoft.AspNetCore.Http;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Application.DTOs.Report;

namespace HostelFinder.Application.Services
{
    public class HostelService : IHostelService
    {
        private readonly IHostelRepository _hostelRepository;
        private readonly IMapper _mapper;
        private readonly IHostelServiceRepository _hostelServiceRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IS3Service _s3Service;

        public HostelService(IHostelRepository hostelRepository, IMapper mapper,
            IHostelServiceRepository hostelServiceRepository, IImageRepository imageRepository, IS3Service s3Service, IAddressRepository addressRepository, IRoomRepository roomRepository)
        {
            _hostelRepository = hostelRepository;
            _mapper = mapper;
            _hostelServiceRepository = hostelServiceRepository;
            _imageRepository = imageRepository;
            _s3Service = s3Service;
            _addressRepository = addressRepository;
            _roomRepository = roomRepository;
        }

        public async Task<Response<HostelResponseDto>> AddHostelAsync(AddHostelRequestDto request, string imageUrl)
        {
            // Kiểm tra trọ có bị trùng địa chỉ không
            var isDuplicate = await _hostelRepository.CheckDuplicateHostelAsync(
                request.HostelName,
                request.Address.Province,
                request.Address.District,
                request.Address.Commune,
                request.Address.DetailAddress
            );

            if (isDuplicate)
            {
                return new Response<HostelResponseDto>("Hostel đã tồn tại với cùng địa chỉ.");
            }

            var hostel = _mapper.Map<Hostel>(request);
            hostel.CreatedOn = DateTime.Now;
            hostel.CreatedBy = request.LandlordId.ToString();

            try
            {
                // Thêm Hostel vào cơ sở dữ liệu
                var hostelAdded = await _hostelRepository.AddAsync(hostel);

                // Thêm các dịch vụ vào Hostel
                foreach (var serviceId in request.ServiceId)
                {
                    HostelServices hostelServices = new HostelServices
                    {
                        ServiceId = serviceId ?? Guid.Empty,
                        HostelId = hostelAdded.Id,
                        CreatedBy = hostelAdded.LandlordId.ToString(),
                        CreatedOn = DateTime.Now,
                        IsDeleted = false,
                    };
                    await _hostelServiceRepository.AddAsync(hostelServices);
                }

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    await _imageRepository.AddAsync(new Image
                    {
                        HostelId = hostelAdded.Id,
                        Url = imageUrl
                    });
                }

                // Map domain object to DTO
                var hostelResponseDto = _mapper.Map<HostelResponseDto>(hostel);

                return new Response<HostelResponseDto>
                {
                    Data = hostelResponseDto,
                    Message = "Thêm trọ mới thành công."
                };
            }
            catch (Exception ex)
            {
                return new Response<HostelResponseDto>(message: ex.Message);
            }
        }


        public async Task<Response<HostelResponseDto>> UpdateHostelAsync(Guid hostelId, UpdateHostelRequestDto request, IFormFile? image)
        {
            var hostel = await _hostelRepository.GetByIdAsync(hostelId);
            if (hostel == null)
            {
                return new Response<HostelResponseDto>("Hostel not found.");
            }

            // Update the basic hostel properties
            _mapper.Map(request, hostel);
            hostel.LastModifiedOn = DateTime.Now;

            try
            {
                using (var transaction = await _hostelRepository.BeginTransactionAsync())
                {
                    // Update or Add Address using AutoMapper
                    var address = await _addressRepository.GetAddressByHostelIdAsync(hostelId);
                    if (address != null)
                    {
                        // Update existing address by mapping request.Address onto it
                        _mapper.Map(request.Address, address);
                        await _addressRepository.UpdateAsync(address);
                    }
                    else
                    {
                        // Map request.Address to a new Address object and add it
                        var newAddress = _mapper.Map<Address>(request.Address);
                        newAddress.HostelId = hostelId;
                        newAddress.CreatedOn = DateTime.Now;
                        await _addressRepository.AddAsync(newAddress);
                    }

                    var existingServices = await _hostelServiceRepository.GetServicesByHostelIdAsync(hostelId);
                    var existingServiceIds = existingServices.Select(s => s.ServiceId).ToList();

                    var newServiceIds = request.ServiceId.Where(id => id.HasValue).Select(id => id.Value).ToList();

                    var servicesToRemove = existingServices.Where(s => !newServiceIds.Contains(s.ServiceId)).ToList();
                    foreach (var service in servicesToRemove)
                    {
                        await _hostelServiceRepository.DeletePermanentAsync(service.Id);
                    }

                    var servicesToAdd = newServiceIds.Except(existingServiceIds).ToList();
                    foreach (var serviceId in servicesToAdd)
                    {
                        var newService = new HostelServices
                        {
                            ServiceId = serviceId,
                            HostelId = hostelId,
                            CreatedBy = hostel.LandlordId.ToString(),
                            CreatedOn = DateTime.Now,
                            IsDeleted = false,
                        };
                        await _hostelServiceRepository.AddAsync(newService);
                    }

                    // Update images
                    if (image != null)
                    {
                        // Xóa hình ảnh cũ
                        var existingImages = await _imageRepository.GetImagesByHostelIdAsync(hostelId);
                        foreach (var existingImage in existingImages)
                        {
                            await _imageRepository.DeletePermanentAsync(existingImage.Id);
                        }

                        // Lưu hình ảnh mới
                        var imageUrl = await _s3Service.UploadFileAsync(image);  // Upload hình ảnh lên S3 (hoặc nơi lưu trữ)
                        var newImage = new Image
                        {
                            HostelId = hostelId,
                            Url = imageUrl,
                            CreatedOn = DateTime.Now,
                        };
                        await _imageRepository.AddAsync(newImage);
                    }


                    // Save hostel details
                    await _hostelRepository.UpdateAsync(hostel);

                    // Commit transaction
                    await transaction.CommitAsync();
                }

                // Map to response DTO
                var hostelResponseDto = _mapper.Map<HostelResponseDto>(hostel);
                return new Response<HostelResponseDto>
                {
                    Data = hostelResponseDto,
                    Message = "Hostel updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new Response<HostelResponseDto>(message: ex.Message);
            }
        }


        public async Task<Response<bool>> DeleteHostelAsync(Guid hostelId)
        {
            var hostel = await _hostelRepository.GetByIdAsync(hostelId);
            if (hostel == null)
            {
                return new Response<bool>(false, "Hostel not found");
            }

            try
            {
                // Kiểm tra xem có phòng nào còn người thuê không
                var roomsWithTenancies = await _roomRepository.GetRoomsByHostelIdAsync(hostelId);  // Lấy danh sách phòng của hostel
                foreach (var room in roomsWithTenancies)
                {
                    // Kiểm tra xem phòng có người thuê không (người thuê còn chưa trả phòng)
                    var roomTenancy = room.RoomTenancies.FirstOrDefault(rt => rt.MoveOutDate == null || rt.MoveOutDate > DateTime.Now);  // Chỉ lấy những roomTenancy chưa có MoveOutDate (người thuê chưa trả phòng)
                    if (roomTenancy != null)
                    {
                        // Nếu có người thuê đang còn trong phòng, không cho phép xóa hostel
                        return new Response<bool>{Succeeded = false, Message = "Nhà trọ vẫn còn phòng thuê, nên không thể xóa", Data = false};
                    }
                }

                await _hostelRepository.DeleteAsync(hostelId);
                return new Response<bool>(true, "Xóa nhà trọ thành công.");
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, message: ex.Message);
            }
        }


        public async Task<PagedResponse<List<ListHostelResponseDto>>> GetHostelsByUserIdAsync(Guid landlordId, string? searchPhrase, int? pageNumber, int? pageSize, string? sortBy, SortDirection? sortDirection)
        {
            try
            {
                var hostels = await _hostelRepository.GetAllMatchingInLandLordAsync(landlordId, searchPhrase, pageSize, pageNumber, sortBy, sortDirection);

                var hostelDtos = _mapper.Map<List<ListHostelResponseDto>>(hostels.Data);
                var pagedResponse = PaginationHelper.CreatePagedResponse(hostelDtos, pageNumber ?? 1,pageSize ?? 10, hostels.TotalRecords);
                return pagedResponse;
            }
            catch (Exception ex)
            {
                return new PagedResponse<List<ListHostelResponseDto>> { Succeeded = false, Errors = { ex.Message } };
            }
        }

        public async Task<Response<HostelResponseDto>> GetHostelByIdAsync(Guid hostelId)
        {
            var hostel = await _hostelRepository.GetHostelByIdAsync(hostelId);
            if (hostel == null)
            {
                return new Response<HostelResponseDto>("Hostel not found.");
            }

            var hostelDto = _mapper.Map<HostelResponseDto>(hostel);

            return new Response<HostelResponseDto>(hostelDto);
        }

        public async Task<PagedResponse<List<ListHostelResponseDto>>> GetAllHostelAsync(GetAllHostelQuery request)
        {
            try
            {
                var hostels = await _hostelRepository.GetAllMatchingAsync(request.SearchPhrase, request.PageSize,
                    request.PageNumber, request.SortBy, request.SortDirection);

                var hostelDtos = _mapper.Map<List<ListHostelResponseDto>>(hostels.Data);

                var pagedResponse = PaginationHelper.CreatePagedResponse(hostelDtos, request.PageNumber,
                    request.PageSize, hostels.TotalRecords);
                return pagedResponse;
            }
            catch (Exception ex)
            {
                return new PagedResponse<List<ListHostelResponseDto>> { Succeeded = false, Errors = { ex.Message } };
            }
        }

        public async Task<DashboardForLandlordResponse> GetDashboardDataAsync(Guid landlordId)
        {
            var dashboardData = new DashboardForLandlordResponse
            {
                HostelCount = await _hostelRepository.GetHostelCountAsync(landlordId),
                TenantCount = await _hostelRepository.GetTenantCountAsync(landlordId),
                RoomCount = await _hostelRepository.GetRoomCountAsync(landlordId),
                OccupiedRoomCount = await _hostelRepository.GetOccupiedRoomCountAsync(landlordId),
                AvailableRoomCount = await _hostelRepository.GetAvailableRoomCountAsync(landlordId),
                AllInvoicesCount = await _hostelRepository.GetAllInvoicesCountAsync(landlordId),
                UnpaidInvoicesCount = await _hostelRepository.GetUnpaidInvoicesCountAsync(landlordId),
                ExpiringContractsCount = await _hostelRepository.GetExpiringContractsCountAsync(landlordId, DateTime.Now),
                PostCount = await _hostelRepository.GetPostCountAsync(landlordId)
            };

            return dashboardData;
        }
    }
}