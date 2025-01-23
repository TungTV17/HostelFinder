namespace HostelFinder.Application.DTOs.Wishlist.Response
{
    public class WishlistResponseDto
    {
        public Guid WishlistId { get; set; }
        public List<WishlistPostResponseDto> Posts { get; set; }
    }
}
