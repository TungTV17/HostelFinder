using HostelFinder.Application.DTOs.Post.Requests;
using HostelFinder.Application.DTOs.Post.Responses;
using HostelFinder.Application.DTOs.Room.Requests;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers;

[ApiController]
[Route("api/posts/")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IS3Service _s3Service;
    private readonly IOpenAiService _openAiService;

    public PostController(IPostService postService, IS3Service s3Service, IOpenAiService openAiService)
    {
        _postService = postService;
        _s3Service = s3Service;
        _openAiService = openAiService;
    }



    [HttpGet("GetAllPostWithPriceAndStatusAndTime")]
    [Authorize(Roles = "Landlord,Admin")]
    public async Task<IActionResult> GetAllPostWithPriceAndStatusAndTime()
    {
        try
        {
            var response = await _postService.GetAllPostWithPriceAndStatusAndTime();

            // Ensure Data is not null and check if it's empty
            if (!response.Succeeded || response.Data == null || !response.Data.Any())
            {
                return NotFound(new Response<List<ListPostsResponseDto>>
                {
                    Succeeded = false,
                    Message = "No posts found",
                    Data = new List<ListPostsResponseDto>() // Initialize empty list
                });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpGet("GetAllPostWithPriceAndStatusAndTimePaging")]
    [Authorize(Roles = "Landlord,Admin")]
    public async Task<IActionResult> GetAllPostWithPriceAndStatusAndTimePaging([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var response = await _postService.GetAllPostWithPriceAndStatusAndTime(pageIndex, pageSize);

            if (!response.Succeeded || response.Data == null || !response.Data.Any())
            {
                return NotFound(new Response<List<ListPostsResponseDto>>
                {
                    Succeeded = false,
                    Message = "No posts found",
                    Data = new List<ListPostsResponseDto>() // Initialize empty list
                });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }


    [HttpPost]
    [Authorize(Roles = "Landlord,Admin")]
    public async Task<IActionResult> AddPost(Guid userId, [FromForm] AddPostRequestDto postDto,
        [FromForm] List<IFormFile> images)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageUrls = new List<string>();

            if (images != null && images.Any())
            {
                foreach (var image in images)
                {
                    try
                    {
                        var imageUrl = await _s3Service.UploadFileAsync(image);
                        imageUrls.Add(imageUrl);
                    }
                    catch (ArgumentException ex)
                    {
                        return BadRequest(new Response<string>
                        {
                            Succeeded = false,
                            Message = $"Image upload failed: {ex.Message}"
                        });
                    }
                }
            }

            var result = await _postService.AddPostAsync(postDto, imageUrls, userId);

            if (!result.Succeeded)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = result.Message
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpPut("{postId}")]
    [Authorize(Roles = "Landlord,Admin")]
    public async Task<IActionResult> UpdatePost(Guid postId, [FromForm] UpdatePostRequestDto request, [FromForm] List<IFormFile>? images, [FromForm] List<string>? imageUrls)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Nếu có hình ảnh, truyền vào request. Nếu không có hình ảnh, truyền null.
            var result = await _postService.UpdatePostAsync(postId, request, images?.Any() == true ? images : null, imageUrls);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Landlord,Admin")]
    [Route("{postId}")]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        try
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized("Người dùng chưa được xác thực.");
            }

            var currentUserId = Guid.Parse(userIdClaim.Value);
            var response = await _postService.DeletePostAsync(postId, currentUserId);
            if (!response.Succeeded)
            {
                return BadRequest(response.Errors);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpPost("get-all")]
    [Authorize(Roles = "Landlord,Admin")]
    public async Task<IActionResult> Get(GetAllPostsQuery request)
    {
        try
        {
            var response = await _postService.GetAllPostAysnc(request);

            if (!response.Succeeded)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Errors = response.Errors
                });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Landlord,Admin")]
    public async Task<IActionResult> GetPostsByUserId(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = "Invalid user ID."
                });
            }

            var result = await _postService.GetPostsByUserIdAsync(userId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(new Response<string>
            {
                Succeeded = false,
                Errors = result.Errors
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpGet("{postId}")]
    public async Task<IActionResult> GetPostById(Guid postId)
    {
        try
        {
            if (postId == Guid.Empty)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = "Invalid post ID."
                });
            }

            var result = await _postService.GetPostByIdAsync(postId);
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(new Response<string>
            {
                Succeeded = false,
                Errors = new List<string> { "Bài đăng không tồn tại." }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpPost("filter")]
    public async Task<IActionResult> FilterPosts([FromForm] FilterPostsRequestDto filter)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = "Invalid filter criteria."
                });
            }

            var result = await _postService.FilterPostsAsync(filter);

            if (!result.Succeeded || result.Data == null || !result.Data.Any())
            {
                return NotFound(new Response<string>
                {
                    Succeeded = false,
                    Message = "No posts found matching the filter criteria."
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpPatch]
    [Authorize(Roles = "Landlord,Admin")]
    [Route("{postId}/push")]
    public async Task<IActionResult> PushPost(Guid postId, Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = "Invalid user ID."
                });
            }

            var result = await _postService.PushPostOnTopAsync(postId, DateTime.Now, userId);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(new Response<string>
            {
                Succeeded = false,
                Message = result.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpGet("ordered")]
    [Authorize(Roles = "Landlord,Admin")]
    public async Task<IActionResult> GetPostsOrderedByPriority()
    {
        try
        {
            var result = await _postService.GetPostsOrderedByPriorityAsync();

            if (!result.Succeeded || result.Data == null || !result.Data.Any())
            {
                return NotFound(new Response<string>
                {
                    Succeeded = false,
                    Message = "No posts available."
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpGet("GetPostsOrderedPaging")]
    [Authorize(Roles = "Landlord,Admin")]
    public async Task<IActionResult> GetPostsOrderedByPriorityPaging([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _postService.GetPostsOrderedByPriorityAsync(pageIndex, pageSize);

            if (!result.Succeeded || result.Data == null || !result.Data.Any())
            {
                return NotFound(new Response<string>
                {
                    Succeeded = false,
                    Message = "No posts available."
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response<string>
            {
                Succeeded = false,
                Message = $"Internal server error: {ex.Message}"
            });
        }
    }

    [HttpPost]
    [Route("generate-description-post")]
    [Authorize(Roles = "Landlord,Admin")]
    public async Task<IActionResult> GenarateDescriptionPost(PostGenerationRequest request)
    {
        try
        {
            var response = await _openAiService.GeneratePostDescriptionsAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("filtered-paged")]
    public async Task<IActionResult> GetFilteredAndPagedPosts(
        [FromForm] FilterPostsRequestDto filter,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        // Kiểm tra tính hợp lệ của pageIndex và pageSize
        if (pageIndex <= 0)
        {
            return BadRequest(new PagedResponse<List<ListPostsResponseDto>>
            {
                Succeeded = false,
                Errors = new List<string> { "PageIndex phải lớn hơn 0." }
            });
        }

        if (pageSize <= 0)
        {
            return BadRequest(new PagedResponse<List<ListPostsResponseDto>>
            {
                Succeeded = false,
                Errors = new List<string> { "PageSize phải lớn hơn 0." }
            });
        }

        try
        {
            // Gọi đến service để lấy dữ liệu
            var pagedPosts = await _postService.GetFilteredAndPagedPostsAsync(filter, pageIndex, pageSize);

            if (!pagedPosts.Succeeded)
            {
                return BadRequest(pagedPosts);
            }

            return Ok(pagedPosts);
        }
        catch (Exception ex)
        {
            // Xử lý ngoại lệ và trả về phản hồi lỗi
            return StatusCode(StatusCodes.Status500InternalServerError, new PagedResponse<List<ListPostsResponseDto>>
            {
                Succeeded = false,
                Errors = new List<string> { "Đã xảy ra lỗi khi xử lý yêu cầu.", ex.Message }
            });
        }
    }

    [HttpGet("top/{topCount}")]
    public async Task<IActionResult> GetTopPosts(int topCount)
    {
        var result = await _postService.GetTopPostsAsync(topCount);

        if (result.Data == null || !result.Data.Any())
        {
            return NotFound("No posts found.");
        }

        return Ok(result);
    }

    [HttpPost("check-user-hostel/{userId}")]
    public async Task<ActionResult<bool>> CheckHostelExist(Guid userId)
    {
        var result = await _postService.CheckUserHostelExist(userId);
        return Ok(result);
    }
}