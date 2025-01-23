
using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.Membership.Requests
{
    public class AddUserMembershipRequestDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid MembershipId { get; set; }
    }

}
