using HostelFinder.Application.DTOs.Membership.Requests;
using HostelFinder.Application.DTOs.Membership.Responses;
using HostelFinder.Application.DTOs.MembershipService.Responses;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IMembershipService
    {
        Task<Response<List<MembershipResponseDto>>> GetAllMembershipWithMembershipService();
        Task<Response<MembershipResponseDto>> AddMembershipAsync(AddMembershipRequestDto membershipDto);
        Task<Response<MembershipResponseDto>> EditMembershipAsync(Guid id, UpdateMembershipRequestDto membershipDto);
        Task<Response<bool>> DeleteMembershipAsync(Guid id);
        Task<Response<string>> UpdatePostCountAsync(Guid userId);
        Task<Response<string>> UpdatePushTopCountAsync(Guid userId);
        Task<Response<List<PostingMemberShipServiceDto>>> GetMembershipServicesForUserAsync(Guid userId);
    }
}
