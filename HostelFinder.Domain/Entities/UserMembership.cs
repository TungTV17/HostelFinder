using HostelFinder.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Domain.Entities
{
    public class UserMembership : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid MembershipId { get; set; }
        public Membership Membership { get; set; }
        public int PostsUsed { get; set; }
        public int PushTopUsed { get; set; }
        [Required]
        public bool IsPaid { get; set; } = false;
        [Required]
        public DateTime StartDate { get; set; } 
        [Required]
        public DateTime ExpiryDate { get; set; } 
    }
}
