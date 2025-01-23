using HostelFinder.Application.DTOs.Wishlist.Request;
using HostelFinder.Application.DTOs.Wishlist.Response;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IWishlistService
    {
        Task<Response<bool>> AddPostToWishlistAsync(AddPostToWishlistRequestDto request);
        Task<Response<WishlistResponseDto>> GetWishlistByUserIdAsync(Guid userId);
        Task<Response<bool>> DeleteRoomFromWishlistAsync(Guid id);
        Task<int> GetWishlistPostCountAsync(Guid userId);
    }
}
