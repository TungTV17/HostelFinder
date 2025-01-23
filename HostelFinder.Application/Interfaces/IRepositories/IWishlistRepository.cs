using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IWishlistRepository : IBaseGenericRepository<Wishlist>
    {
        Task<Wishlist> GetWishlistByUserIdAsync(Guid userId);
        Task AddRoomToWishlistAsync(WishlistPost wishlistRoom);
    }
}
