using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class WishlistRepository : BaseGenericRepository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Wishlist> GetWishlistByUserIdAsync(Guid userId)
        {
            return await _dbContext.Wishlists
                .Include(w => w.WishlistPosts)
                    .ThenInclude(wr => wr.Post)
                        .ThenInclude(p => p.Hostel) 
                        .ThenInclude(h => h.Address) 
                .Include(w => w.WishlistPosts)
                    .ThenInclude(wr => wr.Post)
                        .ThenInclude(p => p.Room)
                .Include(w => w.WishlistPosts)
                    .ThenInclude(wr => wr.Post)
                        .ThenInclude(p => p.MembershipServices) 
                            .ThenInclude(ms => ms.Membership)
                .Include(w => w.WishlistPosts)
                        .ThenInclude(wr => wr.Post)
                            .ThenInclude(p => p.Images)
                 .Where(w => w.UserId == userId && !w.IsDeleted) // Lọc Wishlist
                        .Select(w => new Wishlist
                        {
                            Id = w.Id,
                            UserId = w.UserId,
                            CreatedOn = w.CreatedOn,
                            WishlistPosts = w.WishlistPosts
                                .Where(wp => !wp.Post.IsDeleted && wp.Post.Status == true) // Lọc các Post
                                .Select(wp => new WishlistPost
                                {
                                    Id = wp.Id,
                                    Post = new Post
                                    {
                                        Id = wp.Post.Id,
                                        HostelId = wp.Post.HostelId,
                                        RoomId = wp.Post.RoomId,
                                        Title = wp.Post.Title,
                                        Description = wp.Post.Description,
                                        Status = wp.Post.Status,
                                        CreatedOn = wp.Post.CreatedOn,
                                        IsDeleted = wp.Post.IsDeleted,
                                        Hostel = wp.Post.Hostel != null ? new Hostel
                                        {
                                            Address = wp.Post.Hostel.Address
                                        } : null,
                                        Room = wp.Post.Room,
                                        MembershipServices = wp.Post.MembershipServices != null
                                            ? new MembershipServices
                                            {
                                                Membership = wp.Post.MembershipServices.Membership
                                            }
                                            : null,
                                        Images = wp.Post.Images
                                    }
                                }).ToList()
                        })
                        .OrderByDescending(o => o.CreatedOn)
                        .FirstOrDefaultAsync();
        }


        public async Task AddRoomToWishlistAsync(WishlistPost wishlistRoom)
        {
            await _dbContext.WishlistPosts.AddAsync(wishlistRoom);
            await _dbContext.SaveChangesAsync();
        }

    }
}
