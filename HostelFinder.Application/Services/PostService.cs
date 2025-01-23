using AutoMapper;
using HostelFinder.Application.DTOs.Post.Requests;
using HostelFinder.Application.DTOs.Post.Responses;
using HostelFinder.Application.DTOs.Users.Response;
using HostelFinder.Application.Helpers;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace HostelFinder.Application.Services;

public class PostService : IPostService
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IMembershipService _membershipService;
    private readonly IS3Service _s3Service;

    public PostService(IMapper mapper, IPostRepository postRepository, IUserRepository userRepository,
        IImageRepository imageRepository, IMembershipService membershipService,
        IS3Service s3Service)
    {
        _mapper = mapper;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _imageRepository = imageRepository;
        _membershipService = membershipService;
        _s3Service = s3Service;
    }

    public async Task<Response<bool>> DeletePostAsync(Guid postId, Guid userId)
    {
        var post = await _postRepository.GetPostByIdWithHostelAsync(postId);
        if (post == null)
        {
            return new Response<bool>
            {
                Succeeded = false,
                Errors = ["Bài đăng không tồn tại."]
            };
        }

        if (post.Hostel == null || post.Hostel.LandlordId != userId)
        {
            return new Response<bool>
            {
                Succeeded = false,
                Errors = ["Bạn không có quyền xóa bài đăng này."]
            };
        }

        await _postRepository.DeleteAsync(postId);
        return new Response<bool> { Succeeded = true, Message = "Xóa bài đăng thành công." };
    }

    public async Task<LandlordResponseDto> GetLandlordByPostIdAsync(Guid postId)
    {
        var hostel = await _userRepository.GetHostelByPostIdAsync(postId);

        if (hostel == null || hostel.Landlord == null)
        {
            return null;
        }

        var landlordDto = _mapper.Map<LandlordResponseDto>(hostel.Landlord);

        return landlordDto;
    }

    public async Task<PagedResponse<List<ListPostsResponseDto>>> GetAllPostAysnc(GetAllPostsQuery request)
    {
        try
        {
            var posts = await _postRepository.GetAllMatchingAsync(request.SearchPhrase, request.PageSize,
                request.PageNumber, request.SortBy, request.SortDirection);

            var postsDtos = _mapper.Map<List<ListPostsResponseDto>>(posts.Data);

            var pagedResponse = PaginationHelper.CreatePagedResponse(postsDtos, request.PageNumber, request.PageSize,
                posts.TotalRecords);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            return new PagedResponse<List<ListPostsResponseDto>> { Succeeded = false, Errors = { ex.Message } };
        }
    }

    public async Task<Response<PostResponseDto>> GetPostByIdAsync(Guid postId)
    {
        var post = await _postRepository.GetPostByIdAsync(postId);

        if (post == null)
        {
            return new Response<PostResponseDto>
            {
                Succeeded = false,
                Errors = new List<string> { "Bài đăng không tồn tại." }
            };
        }

        var postDto = _mapper.Map<PostResponseDto>(post);
        return new Response<PostResponseDto>
        {
            Data = postDto,
            Succeeded = true,
        };
    }

    public async Task<Response<List<ListPostsResponseDto>>> GetPostsByUserIdAsync(Guid userId)
    {
        var posts = await _postRepository.GetPostsByUserIdAsync(userId);

        if (posts == null || !posts.Any())
        {
            return new Response<List<ListPostsResponseDto>>
            {
                Succeeded = false,
                Errors = new List<string> { "Bạn chưa có bài đăng nào." }
            };
        }

        var postDtos = _mapper.Map<List<ListPostsResponseDto>>(posts);
        return new Response<List<ListPostsResponseDto>>
        {
            Data = postDtos,
            Succeeded = true
        };
    }

    public async Task<Response<AddPostRequestDto>> AddPostAsync(AddPostRequestDto request, List<string> imageUrls,
        Guid userId)
    {
        if (_membershipService == null)
        {
            return new Response<AddPostRequestDto>
            {
                Succeeded = false,
                Message = "Membership service not initialized."
            };
        }

        var postCountResponse = await _membershipService.UpdatePostCountAsync(userId);

        if (!postCountResponse.Succeeded)
        {
            return new Response<AddPostRequestDto>
            {
                Succeeded = false,
                Message = postCountResponse.Message
            };
        }

        var post = _mapper.Map<Post>(request);
        post.CreatedBy = userId.ToString();
        post.CreatedOn = DateTime.Now;

        try
        {
            using (var transaction = await _postRepository.BeginTransactionAsync())
            {
                await _postRepository.AddAsync(post);

                foreach (var imageUrl in imageUrls)
                {
                    await _imageRepository.AddAsync(new Image
                    {
                        PostId = post.Id,
                        HostelId = post.HostelId,
                        Url = imageUrl,
                        CreatedOn = DateTime.Now,
                    });
                }

                await transaction.CommitAsync();
            }

            var postResponseDto = _mapper.Map<AddPostRequestDto>(post);
            return new Response<AddPostRequestDto>
            {
                Data = postResponseDto,
                Succeeded = true,
                Message = "Thêm bài đăng thành công."
            };
        }
        catch (Exception ex)
        {
            return new Response<AddPostRequestDto>
            {
                Succeeded = false,
                Message = ex.Message
            };
        }
    }

    public async Task<Response<List<ListPostsResponseDto>>> FilterPostsAsync(FilterPostsRequestDto filter)
    {
        var posts = await _postRepository.FilterPostsAsync(
            filter.Province,
            filter.District,
            filter.Commune,
            filter.MinSize,
            filter.MaxSize,
            filter.MinPrice,
            filter.MaxPrice,
            filter.RoomType
        );

        var postDtos = _mapper.Map<List<ListPostsResponseDto>>(posts);
        return new Response<List<ListPostsResponseDto>>
        {
            Data = postDtos,
            Succeeded = true
        };
    }

    public async Task<Response<PostResponseDto>> PushPostOnTopAsync(Guid postId, DateTime newDate, Guid userId)
    {
        if (_membershipService == null)
        {
            return new Response<PostResponseDto>
            {
                Succeeded = false,
                Message = "Membership service not initialized."
            };
        }

        var pushCountResponse = await _membershipService.UpdatePushTopCountAsync(userId);
        if (!pushCountResponse.Succeeded)
        {
            return new Response<PostResponseDto>
            {
                Succeeded = false,
                Message = pushCountResponse.Message
            };
        }

        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            return new Response<PostResponseDto>
            {
                Succeeded = false,
                Message = "Không tìm thấy bài đăng."
            };
        }

        try
        {
            using (var transaction = await _postRepository.BeginTransactionAsync())
            {
                post.CreatedOn = newDate;
                post.LastModifiedOn = newDate;
                await _postRepository.UpdateAsync(post);

                await transaction.CommitAsync();
            }

            var postResponseDto = _mapper.Map<PostResponseDto>(post);
            return new Response<PostResponseDto>
            {
                Succeeded = true,
                Message = "Đẩy bài đăng lên thành công.",
                Data = postResponseDto
            };
        }
        catch (Exception ex)
        {
            return new Response<PostResponseDto>
            {
                Succeeded = false,
                Message = ex.Message
            };
        }
    }

    public async Task<Response<List<ListPostsResponseDto>>> GetPostsOrderedByPriorityAsync()
    {
        var posts = await _postRepository.GetPostsOrderedByMembershipPriceAndCreatedOnAsync();
        var postDtos = _mapper.Map<List<ListPostsResponseDto>>(posts);

        return new Response<List<ListPostsResponseDto>>
        {
            Data = postDtos,
            Succeeded = true
        };
    }

    public async Task<PagedResponse<List<ListPostsResponseDto>>> GetPostsOrderedByPriorityAsync(int pageIndex,
        int pageSize)
    {
        var pagedPosts = await _postRepository.GetPostsOrderedByMembershipPriceAndCreatedOnAsync(pageIndex, pageSize);

        var postDtos = _mapper.Map<List<ListPostsResponseDto>>(pagedPosts.Data);

        return PaginationHelper.CreatePagedResponse(
            postDtos,
            pageIndex,
            pageSize,
            pagedPosts.TotalRecords
        );
    }

    public async Task<Response<PostResponseDto>> UpdatePostAsync(Guid postId, UpdatePostRequestDto request,
     List<IFormFile>? images, List<string>? imageUrls)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            return new Response<PostResponseDto>
            {
                Succeeded = false,
                Message = "Post not found."
            };
        }

        // Mapping updated data to post object
        _mapper.Map(request, post);
        post.LastModifiedOn = DateTime.Now;

        try
        {
            await using (var transaction = await _postRepository.BeginTransactionAsync())
            {
                // Update post in repository
                await _postRepository.UpdateAsync(post);

                // Fetch existing images associated with the post
                var existingImages = await _imageRepository.GetImagesByPostIdAsync(postId);

                // List to hold new image URLs for adding
                var newImageUrls = new List<string>();

                // Nếu có hình ảnh mới, tải lên và thêm vào
                if (images != null && images.Any())
                {
                    foreach (var image in images)
                    {
                        try
                        {
                            // Upload image and get URL
                            var imageUrl = await _s3Service.UploadFileAsync(image);
                            newImageUrls.Add(imageUrl);
                        }
                        catch (Exception ex)
                        {
                            // In case of image upload failure, return an error message
                            return new Response<PostResponseDto>
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
                            // You may want to skip images that are already present
                            var imageFromUrl = await _s3Service.UploadFileFromUrlAsync(item);
                            newImageUrls.Add(imageFromUrl);
                        }
                        catch (Exception ex)
                        {
                            // In case of URL upload failure, return an error message
                            return new Response<PostResponseDto>
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
                        PostId = post.Id,
                        HostelId = post.HostelId,
                        Url = imageUrl,
                        CreatedOn = DateTime.Now,
                    });
                }

                // Commit transaction
                await transaction.CommitAsync();
            }

            // Map the updated post to response DTO
            var postResponseDto = _mapper.Map<PostResponseDto>(post);
            return new Response<PostResponseDto>
            {
                Data = postResponseDto,
                Succeeded = true,
                Message = "Cập nhật bài đăng thành công."
            };
        }
        catch (Exception ex)
        {
            // Handle general exception
            return new Response<PostResponseDto>
            {
                Succeeded = false,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }

    public async Task<Response<List<ListPostsResponseDto>>> GetAllPostWithPriceAndStatusAndTime()
    {
        // Fetch posts from the repository
        var posts = await _postRepository.GetAllPostsOrderedAsync();

        // Map posts to DTOs or initialize with an empty list if null/empty
        var postDtos = posts.Any()
            ? _mapper.Map<List<ListPostsResponseDto>>(posts)
            : new List<ListPostsResponseDto>();

        // Return the response with appropriate success status
        return new Response<List<ListPostsResponseDto>>
        {
            Data = postDtos,
            Succeeded = postDtos.Any(),
            Message = postDtos.Any() ? "Posts retrieved successfully" : "No posts found"
        };
    }

    public async Task<PagedResponse<List<ListPostsResponseDto>>> GetAllPostWithPriceAndStatusAndTime(int pageIndex,
        int pageSize)
    {
        var pagedPosts = await _postRepository.GetAllPostsOrderedAsync(pageIndex, pageSize);

        var postDtos = _mapper.Map<List<ListPostsResponseDto>>(pagedPosts.Data);

        return PaginationHelper.CreatePagedResponse(
            postDtos,
            pageIndex,
            pageSize,
            pagedPosts.TotalRecords
        );
    }

    public async Task<PagedResponse<List<ListPostsResponseDto>>> GetFilteredAndPagedPostsAsync(FilterPostsRequestDto filter, int pageIndex, int pageSize)
    {
        var posts = await _postRepository.GetFilteredAndPagedPostsAsync(filter, pageIndex, pageSize);

        var postDtos = _mapper.Map<List<ListPostsResponseDto>>(posts.Data);

        return PaginationHelper.CreatePagedResponse(
            postDtos,
            pageIndex,
            pageSize,
            posts.TotalPages
        );
    }

    public async Task<Response<List<ListPostsResponseDto>>> GetTopPostsAsync(int topCount)
    {
        // Get the top posts from the repository
        var posts = await _postRepository.GetTopPostsAsync(topCount);
        var postDtos = _mapper.Map<List<ListPostsResponseDto>>(posts);

        return new Response<List<ListPostsResponseDto>>(postDtos);
    }

    public async Task<Response<bool>> CheckUserHostelExist(Guid userId)
    {
        var exists = await _postRepository.CheckUserHostelExist(userId);
        if (!exists)
        {
            return new Response<bool>
            {
                Succeeded = false,
                Message = "Bạn chưa có phòng trọ nào. Vui lòng thêm nhà trọ và phòng trọ trước!"
            };
        }
        
        return new Response<bool>{ Succeeded = true, Message = "Hostel found." };
    }
}