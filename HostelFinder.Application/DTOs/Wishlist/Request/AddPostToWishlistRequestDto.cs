namespace HostelFinder.Application.DTOs.Wishlist.Request
{
    public class AddPostToWishlistRequestDto
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}
