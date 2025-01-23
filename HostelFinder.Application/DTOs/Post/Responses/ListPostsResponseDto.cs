using HostelFinder.Application.DTOs.Address;

namespace HostelFinder.Application.DTOs.Post.Responses;

public class ListPostsResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public AddressDto Address { get; set; }
    public decimal MonthlyRentCost { get; set; }
    public decimal Size { get; set; }
    public string MembershipTag { get; set; }
    public string FirstImage { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public bool Status { get; set; }
}