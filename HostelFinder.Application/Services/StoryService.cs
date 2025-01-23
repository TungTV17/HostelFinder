using AutoMapper;
using HostelFinder.Application.DTOs.Story.Requests;
using HostelFinder.Application.DTOs.Story.Responses;
using HostelFinder.Application.Helpers;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace HostelFinder.Application.Services
{
    public class StoryService : IStoryService
    {
        private readonly IStoryRepository _storyRepository;
        private readonly IAddressStoryRepository _addressStoryRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;

        public StoryService(IStoryRepository storyRepository, IAddressStoryRepository addressStoryRepository,
            IImageRepository imageRepository, IMapper mapper, IS3Service s3Service)
        {
            _storyRepository = storyRepository;
            _addressStoryRepository = addressStoryRepository;
            _imageRepository = imageRepository;
            _mapper = mapper;
            _s3Service = s3Service;
        }

        public async Task<Response<string>> AddStoryAsync(AddStoryRequestDto request)
        {
            try
            {
                var storyCountToday = await _storyRepository.CountStoriesByUserTodayAsync(request.UserId);

                if (storyCountToday >= 5)
                {
                    return new Response<string>
                    {
                        Succeeded = false,
                        Message = "Bạn đã đăng đủ 5 bài hôm nay. Vui lòng thử lại vào ngày mai."
                    };
                }

                var story = _mapper.Map<Story>(request);
                story.CreatedBy = request.UserId.ToString();
                story.CreatedOn = DateTime.Now;
                var address = _mapper.Map<AddressStory>(request.AddressStory);
                story.AddressStory = address;

                await _addressStoryRepository.AddAsync(address);
                await _storyRepository.AddAsync(story);

                var images = new List<Image>();
                if (request.Images != null && request.Images.Any())
                {
                    foreach (var imageFile in request.Images)
                    {
                        var imageUrl = await _s3Service.UploadFileAsync(imageFile);
                        var image = new Image
                        {
                            Url = imageUrl,
                            StoryId = story.Id
                        };
                        images.Add(image);
                    }
                }

                foreach (var image in images)
                {
                    await _imageRepository.AddAsync(image);
                }

                story.Images = images;

                return new Response<string>
                {
                    Succeeded = true,
                    Message = "Thêm bài viết thành công",
                    Data = story.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                return new Response<string>
                {
                    Succeeded = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<Response<StoryResponseDto>> GetStoryByIdAsync(Guid id)
        {
            try
            {
                var story = await _storyRepository.GetStoryByIdAsync(id);

                if (story == null)
                {
                    return new Response<StoryResponseDto>
                    {
                        Succeeded = false,
                        Message = "Bài viết không tồn tại."
                    };
                }

                var storyDto = _mapper.Map<StoryResponseDto>(story);

                return new Response<StoryResponseDto>
                {
                    Succeeded = true,
                    Message = "Lấy bài viết thành công.",
                    Data = storyDto
                };
            }
            catch (Exception ex)
            {
                return new Response<StoryResponseDto>
                {
                    Succeeded = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<PagedResponse<List<ListStoryResponseDto>>> GetAllStoriesAsync(int pageIndex, int pageSize,
            StoryFilterDto filter)
        {
            try
            {
                var pagedStories = await _storyRepository.GetAllStoriesAsync(pageIndex, pageSize, filter);
                var storyDtos = _mapper.Map<List<ListStoryResponseDto>>(pagedStories.Data);

                return PaginationHelper.CreatePagedResponse(storyDtos, pageIndex, pageSize, pagedStories.TotalRecords);
            }
            catch (Exception ex)
            {
                return new PagedResponse<List<ListStoryResponseDto>>
                {
                    Succeeded = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<PagedResponse<List<ListStoryResponseDto>>> GetAllStoryForAdminAsync(int pageIndex,
            int pageSize)
        {
            try
            {
                // Gọi phương thức phân trang từ repository
                var pagedStories = await _storyRepository.GetAllStoriesNoCondition(pageIndex, pageSize);
                var storyDtos = _mapper.Map<List<ListStoryResponseDto>>(pagedStories.Data);

                return PaginationHelper.CreatePagedResponse(storyDtos, pageIndex, pageSize, pagedStories.TotalRecords);
            }
            catch (Exception ex)
            {
                return new PagedResponse<List<ListStoryResponseDto>>
                {
                    Succeeded = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<PagedResponse<List<ListStoryResponseDto>>> GetStoryByUserIdAsync(Guid userId, int pageIndex,
            int pageSize)
        {
            try
            {
                // Lấy bài viết phân trang theo userId
                var pagedStories = await _storyRepository.GetStoriesByUserId(userId, pageIndex, pageSize);

                if (!pagedStories.Data.Any())
                {
                    return new PagedResponse<List<ListStoryResponseDto>>
                    {
                        Succeeded = false,
                        Message = "Không có bài viết nào của người dùng này.",
                        Data = null
                    };
                }

                var storyDtos = _mapper.Map<List<ListStoryResponseDto>>(pagedStories.Data);

                return PaginationHelper.CreatePagedResponse(storyDtos, pageIndex, pageSize, pagedStories.TotalRecords);
            }
            catch (Exception ex)
            {
                return new PagedResponse<List<ListStoryResponseDto>>
                {
                    Succeeded = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<Response<string>> DeleteStoryAsync(Guid storyId)
        {
            try
            {
                await _storyRepository.DeleteAsync(storyId);
                return new Response<string>
                {
                    Succeeded = true,
                    Message = "Xóa bài viết thành công.",
                    Data = storyId.ToString()
                };
            }
            catch (Exception ex)
            {
                return new Response<string>
                {
                    Succeeded = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<Response<StoryResponseDto>> UpdateStoryAsync(Guid storyId, UpdateStoryRequestDto request,
            List<IFormFile>? images, List<string>? imageUrls)
        {
            var story = await _storyRepository.GetStoryByIdAsync(storyId);
            if (story == null)
            {
                return new Response<StoryResponseDto>("Story not found.");
            }

            _mapper.Map(request, story);
            story.LastModifiedOn = DateTime.Now;

            try
            {
                var address = await _addressStoryRepository.GetAddressByStoryIdAsync(storyId);
                if (address != null)
                {
                    _mapper.Map(request.AddressStory, address);
                    await _addressStoryRepository.UpdateAsync(address);
                }
                else
                {
                    var newAddress = _mapper.Map<AddressStory>(request.AddressStory);
                    newAddress.StoryId = storyId;
                    newAddress.CreatedOn = DateTime.Now;
                    await _addressStoryRepository.AddAsync(newAddress);
                }

                var existingImages = await _imageRepository.GetImagesByStoryIdAsync(storyId);
                var newImageUrls = new List<string>();

                // Nếu có hình ảnh mới, tải lên và thêm vào
                if (images != null && images.Any())
                {
                    foreach (var image in images)
                    {
                        try
                        {
                            var imageUrl = await _s3Service.UploadFileAsync(image);
                            newImageUrls.Add(imageUrl);
                        }
                        catch (Exception ex)
                        {
                            return new Response<StoryResponseDto>
                            {
                                Succeeded = false,
                                Message = $"Image upload failed: {ex.Message}"
                            };
                        }
                    }
                }

                // Nếu có hình ảnh URL mới, thêm vào
                if (imageUrls != null && imageUrls.Any())
                {
                    foreach (var item in imageUrls)
                    {
                        try
                        {
                            var imageFromUrl = await _s3Service.UploadFileFromUrlAsync(item);
                            newImageUrls.Add(imageFromUrl);
                        }
                        catch (Exception ex)
                        {
                            return new Response<StoryResponseDto>
                            {
                                Succeeded = false,
                                Message = $"URL image upload failed: {ex.Message}"
                            };
                        }
                    }
                }

                // Xóa hình ảnh cũ nếu có hình ảnh mới được tải lên (chỉ xóa nếu có ảnh mới)
                if (newImageUrls.Any())
                {
                    foreach (var item in existingImages)
                    {
                        await _imageRepository.DeletePermanentAsync(item.Id);
                    }
                }

                // Thêm hình ảnh mới vào kho lưu trữ
                foreach (var imageUrl in newImageUrls)
                {
                    await _imageRepository.AddAsync(new Image
                    {
                        StoryId = story.Id,
                        Url = imageUrl,
                        CreatedOn = DateTime.Now,
                    });
                }

                await _storyRepository.UpdateAsync(story);
                var storyResponseDto = _mapper.Map<StoryResponseDto>(story);
                return new Response<StoryResponseDto>
                {
                    Data = storyResponseDto,
                    Message = "Cập nhật bài viết thành công."
                };
            }
            catch (Exception ex)
            {
                return new Response<StoryResponseDto>(message: ex.Message);
            }
        }

        public async Task<Response<StoryResponseDto>> DenyStoryAsync(Guid storyId)
        {
            var story = await _storyRepository.GetStoryByIdAsync(storyId);
            if (story == null)
            {
                return new Response<StoryResponseDto>("Story not found.");
            }

            try
            {
                story.BookingStatus = BookingStatus.Denied;
                story.CreatedOn = DateTime.Now;

                await _storyRepository.UpdateAsync(story);

                var storyResponseDto = _mapper.Map<StoryResponseDto>(story);
                return new Response<StoryResponseDto>
                {
                    Data = storyResponseDto,
                    Message = "Bài đăng đã được từ chối."
                };
            }
            catch (Exception ex)
            {
                return new Response<StoryResponseDto>(message: ex.Message);
            }
        }

        public async Task<Response<StoryResponseDto>> AcceptStoryAsync(Guid storyId)
        {
            var story = await _storyRepository.GetStoryByIdAsync(storyId);
            if (story == null)
            {
                return new Response<StoryResponseDto>("Story not found.");
            }

            try
            {
                story.BookingStatus = BookingStatus.Accepted;
                story.CreatedOn = DateTime.Now;

                await _storyRepository.UpdateAsync(story);

                var storyResponseDto = _mapper.Map<StoryResponseDto>(story);
                return new Response<StoryResponseDto>
                {
                    Data = storyResponseDto,
                    Message = "Bài đăng được chấp nhận."
                };
            }
            catch (Exception ex)
            {
                return new Response<StoryResponseDto>(message: ex.Message);
            }
        }
    }
}