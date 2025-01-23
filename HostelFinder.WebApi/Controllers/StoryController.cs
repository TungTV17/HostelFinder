using HostelFinder.Application.DTOs.Story.Requests;
using HostelFinder.Application.DTOs.Story.Responses;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryController : ControllerBase
    {

        private readonly IStoryService _storyService;

        public StoryController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddStory([FromForm] AddStoryRequestDto request)
        {
            var response = await _storyService.AddStoryAsync(request);
            if (response.Succeeded)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoryById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "ID không hợp lệ." });
            }

            try
            {
                var storyResponse = await _storyService.GetStoryByIdAsync(id);

                if (storyResponse == null || storyResponse.Data == null || !storyResponse.Succeeded)
                {
                    var errorResponse = new Response<StoryResponseDto>
                    {
                        Succeeded = false,
                        Message = "Bài đăng không tìm thấy."
                    };
                    return NotFound(errorResponse);
                }

                return Ok(storyResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new Response<string>
                {
                    Succeeded = false,
                    Message = $"Lỗi: {ex.Message}"
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpPost("GetAllStoryIndex")]
        public async Task<IActionResult> GetAllStoriesAsync([FromForm] StoryFilterDto filter, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _storyService.GetAllStoriesAsync(pageIndex, pageSize, filter);

                if (response.Succeeded)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<PagedResponse<ListStoryResponseDto>>
                {
                    Succeeded = false,
                    Message = $"Lỗi server: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpGet("GetAllStoryForAdmin")]
        public async Task<IActionResult> GetAllStoryForAdmin([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedResponse = await _storyService.GetAllStoryForAdminAsync(pageIndex, pageSize);

                if (pagedResponse.Succeeded)
                {
                    return Ok(pagedResponse);
                }

                return BadRequest(pagedResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<PagedResponse<ListStoryResponseDto>>
                {
                    Succeeded = false,
                    Message = $"Lỗi server: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetStoryByUserId(Guid userId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _storyService.GetStoryByUserIdAsync(userId, pageIndex, pageSize);

                if (response.Succeeded)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<PagedResponse<ListStoryResponseDto>>
                {
                    Succeeded = false,
                    Message = $"Lỗi server: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpDelete("{storyId}")]
        public async Task<IActionResult> DeleteStory(Guid storyId)
        {
            try
            {
                var response = await _storyService.DeleteStoryAsync(storyId);

                if (response.Succeeded)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Succeeded = false,
                    Message = $"Lỗi server: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpPut("{storyId}")]
        public async Task<IActionResult> UpdateStory(Guid storyId, [FromForm] UpdateStoryRequestDto request, [FromForm] List<IFormFile>? images, [FromForm] List<string>? imageUrls)
        {
            if (request == null)
            {
                return BadRequest(new Response<StoryResponseDto>("Invalid input data."));
            }

            try
            {
                var response = await _storyService.UpdateStoryAsync(storyId, request, images, imageUrls);

                if (response.Succeeded)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<StoryResponseDto>
                {
                    Succeeded = false,
                    Message = $"Lỗi server: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpPatch("deny/{storyId}")]
        public async Task<IActionResult> DenyStory(Guid storyId)
        {
            try
            {
                var response = await _storyService.DenyStoryAsync(storyId);

                if (response.Succeeded)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<StoryResponseDto>
                {
                    Succeeded = false,
                    Message = $"Lỗi server: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpPatch("accept/{storyId}")]
        public async Task<IActionResult> AcceptStory(Guid storyId)
        {
            try
            {
                var response = await _storyService.AcceptStoryAsync(storyId);
                if (response.Succeeded)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new Response<StoryResponseDto>
                {
                    Succeeded = false,
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
                return StatusCode(500, errorResponse);
            }
        }

    }
}
