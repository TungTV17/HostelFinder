using HostelFinder.Application.DTOs.Wishlist.Request;
using HostelFinder.Application.DTOs.Wishlist.Response;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelFinder.WebApi.Controllers
{
    [Route("api/wishlists")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpPost("AddRoomToWishlist")]
        [Authorize(Roles = "User, Landlord")]
        public async Task<IActionResult> AddRoomToWishlist([FromBody] AddPostToWishlistRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid || request.PostId == Guid.Empty || request.UserId == Guid.Empty)
                {
                    return BadRequest(new Response<bool>
                    {
                        Succeeded = false,
                        Message = "Invalid Post ID or User ID."
                    });
                }

                var result = await _wishlistService.AddPostToWishlistAsync(request);

                if (!result.Succeeded)
                {
                    return BadRequest(new Response<bool>
                    {
                        Succeeded = false,
                        Errors = result.Errors
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<bool>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        // GET: api/Wishlist/GetWishlistByUserId/{userId}
        [HttpGet("GetWishlistByUserId/{userId}")]
        public async Task<IActionResult> GetWishlistByUserId(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return BadRequest(new Response<WishlistResponseDto>
                    {
                        Succeeded = false,
                        Message = "Invalid User ID."
                    });
                }

                var result = await _wishlistService.GetWishlistByUserIdAsync(userId);
                if (!result.Succeeded || result.Data == null)
                {
                    return NotFound(new Response<WishlistResponseDto>
                    {
                        Succeeded = false,
                        Message = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<WishlistResponseDto>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        // DELETE: api/Wishlist/DeleteRoomFromWishlist
        [HttpDelete("DeleteRoomFromWishlist")]
        public async Task<IActionResult> DeleteWishlist(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new Response<bool>
                    {
                        Succeeded = false,
                        Message = "Invalid ID."
                    });
                }

                var result = await _wishlistService.DeleteRoomFromWishlistAsync(id);

                if (!result.Succeeded)
                {
                    return NotFound(new Response<bool>
                    {
                        Succeeded = false,
                        Message = result.Message
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<bool>
                {
                    Succeeded = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        // GET: api/wishlist/count/{userId}
        [HttpGet("count/{userId}")]
        public async Task<IActionResult> GetWishlistPostCountAsync(Guid userId)
        {
            try
            {
                // Lấy số lượng bài viết trong Wishlist của người dùng
                var count = await _wishlistService.GetWishlistPostCountAsync(userId);

                // Trả về số lượng dưới dạng JSON
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}