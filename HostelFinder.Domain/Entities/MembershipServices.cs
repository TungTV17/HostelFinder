using HostelFinder.Domain.Common;
using RoomFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class MembershipServices : BaseEntity
    {
        public string ServiceName {  get; set; }
        public Guid MembershipId { get; set; }
        public int? MaxPostsAllowed { get; set; }
        public int? MaxPushTopAllowed { get; set; }
        public virtual Membership Membership { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}