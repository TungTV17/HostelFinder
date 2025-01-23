using HostelFinder.Application.DTOs.MembershipService.Responses;

namespace HostelFinder.Application.DTOs.Membership.Responses
{
    public class MembershipResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public MembershipServiceResponseDto MembershipServices { get; set; }
    }
}
