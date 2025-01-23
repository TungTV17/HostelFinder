using HostelFinder.Application.Common;
using HostelFinder.Application.DTOs.MembershipService.Requests;
using HostelFinder.Application.Services;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IMembershipRepository : IBaseGenericRepository<Membership>
    {
        Task Add(MembershipServices entity);
        void Update(MembershipServices entity);
        Task<IEnumerable<Membership>> GetAllMembershipWithMembershipService();
        Task<Membership> GetMembershipWithServicesAsync(Guid id);
        Task<bool> CheckDuplicateMembershipAsync(string name, string description);
        Task AddMembershipWithServiceAsync(Membership membership, AddMembershipServiceReqDto membershipServices);
        Task<List<MembershipServices?>> GetMembershipServicesByMembershipIdAsync(Guid membershipId);
        Task<List<UserMembership>> GetUserMembershipsAsync(Guid userId);
        Task<List<MembershipServices>> GetMembershipServicesByUserAsync(Guid userId);
    }
}
