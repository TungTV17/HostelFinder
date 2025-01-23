using HostelFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class Wishlist : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<WishlistPost> WishlistPosts { get; set; }
    }
}
