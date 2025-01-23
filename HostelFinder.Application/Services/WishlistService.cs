using AutoMapper;
using Google.Apis.Util;
using HostelFinder.Application.DTOs.Address;
using HostelFinder.Application.DTOs.Post.Responses;
using HostelFinder.Application.DTOs.Room.Requests;
using HostelFinder.Application.DTOs.Wishlist.Request;
using HostelFinder.Application.DTOs.Wishlist.Response;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Services
{
    public class WishlistService : IWishlistService
    {

        private readonly IWishlistRepository _wishlistRepository;
        private readonly IWishlistPostRepository _wishlistPostRepository;
        private readonly IMapper _mapper;

        public WishlistService(IWishlistRepository wishlistRepository, IWishlistPostRepository wishlistPostRepository, IMapper mapper)
        {
            _wishlistRepository = wishlistRepository;
            _wishlistPostRepository = wishlistPostRepository;
            _mapper = mapper;
        }

        public async Task<Response<bool>> AddPostToWishlistAsync(AddPostToWishlistRequestDto request)
        {
            if (request.PostId == Guid.Empty || request.UserId == Guid.Empty)
            {
                return new Response<bool>
                {
                    Succeeded = false,
                    Errors = new List<string> { "Invalid Post ID or User ID." }
                };
            }

            var wishlist = await _wishlistRepository.GetWishlistByUserIdAsync(request.UserId);
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = request.UserId,
                    CreatedOn = DateTime.Now,
                    WishlistPosts = new List<WishlistPost>()
                };
                await _wishlistRepository.AddAsync(wishlist);
            }

            var wishlistRoom = new WishlistPost
            {
                WishlistId = wishlist.Id,
                PostId = request.PostId,
                CreatedOn = DateTime.Now
            };

            await _wishlistRepository.AddRoomToWishlistAsync(wishlistRoom);

            return new Response<bool>(true, "Thêm vào danh sách yêu thích thành công.");
        }



        public async Task<Response<WishlistResponseDto>> GetWishlistByUserIdAsync(Guid userId)
        {
            var wishlist = await _wishlistRepository.GetWishlistByUserIdAsync(userId);
            if (wishlist == null)
            {
                return new Response<WishlistResponseDto>(null, "Wishlist not found");
            }

            var response = new WishlistResponseDto
            {
                WishlistId = wishlist.Id,
                Posts = wishlist.WishlistPosts.Select(wr => new WishlistPostResponseDto
                {
                    Id = wr.Post.Id,
                    HostelId = wr.Post.HostelId,  
                    RoomId = wr.Post.RoomId,
                    WishlistPostId = wr.Id,
                    Title = wr.Post.Title, 
                    Description = wr.Post.Description, 
                    Address = new AddressDto
                    {
                        Province = wr.Post.Hostel.Address.Province,  
                        District = wr.Post.Hostel.Address.District,  
                        Commune = wr.Post.Hostel.Address.Commune,  
                        DetailAddress = wr.Post.Hostel.Address.DetailAddress
                    },
                    MonthlyRentCost = wr.Post.Room.MonthlyRentCost,  
                    Size = wr.Post.Room.Size, 
                    FirstImage = wr.Post.Images.Any() ? wr.Post.Images.First().Url : null, 
                    MembershipTag = wr.Post.MembershipServices?.Membership?.Tag, 
                    CreatedOn = wr.Post.CreatedOn,  
                    Status = wr.Post.Status  
                }).ToList()  
            };
            return new Response<WishlistResponseDto>(response);
        }

        public async Task<int> GetWishlistPostCountAsync(Guid userId)
        {
            var wishlist = await _wishlistRepository.GetWishlistByUserIdAsync(userId);
            if (wishlist == null)
            {
                throw new Exception("Wishlist not found.");
            }

            return wishlist.WishlistPosts.Count;
        }


        public async Task<Response<bool>> DeleteRoomFromWishlistAsync(Guid id)
        {
            var wishlist = await _wishlistPostRepository.GetByIdAsync(id);
            if (wishlist == null)
            {
                return new Response<bool>(false, "Wishlist not found");
            }

            await _wishlistPostRepository.DeletePermanentAsync(wishlist.Id);
            return new Response<bool>(true, "Đã xóa khỏi danh sách yêu thích.");
        }

    }
}
