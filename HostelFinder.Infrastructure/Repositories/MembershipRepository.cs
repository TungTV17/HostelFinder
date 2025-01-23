using DocumentFormat.OpenXml.InkML;
using HostelFinder.Application.DTOs.Membership.Responses;
using HostelFinder.Application.DTOs.MembershipService.Requests;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Services;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class MembershipRepository : BaseGenericRepository<Membership>, IMembershipRepository
    {
        public MembershipRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipWithMembershipService()
        {
            return await _dbContext.Memberships.
                    Include(mb => mb.MembershipServices)
                    .Where (mb => !mb.IsDeleted)
                    .ToListAsync();
        }

        public async Task AddMembershipWithServiceAsync(Membership membership, AddMembershipServiceReqDto membershipServiceDto)
        {
            // Tạo một đối tượng MembershipService từ DTO
            var membershipService = new MembershipServices
            {
                ServiceName = membershipServiceDto.ServiceName,
                MaxPostsAllowed = membershipServiceDto.MaxPostsAllowed,
                MaxPushTopAllowed = membershipServiceDto.MaxPushTopAllowed,
                Membership = membership,
                CreatedOn = DateTime.Now,
                CreatedBy = "System",
            };

            // Gán dịch vụ cho Membership
            membership.MembershipServices = new List<MembershipServices> { membershipService };

            // Thêm Membership cùng dịch vụ liên quan vào cơ sở dữ liệu
            await _dbContext.Memberships.AddAsync(membership);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<bool> CheckDuplicateMembershipAsync(string name, string description)
        {
            return await _dbContext.Memberships
                .AnyAsync(mb => mb.Name == name && mb.Description == description);
        }

        public async Task<Membership> GetMembershipWithServicesAsync(Guid id)
        {
            var membership = await _dbContext.Memberships
                .Include(m => m.MembershipServices)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
            return membership;
        }

        public void Update(MembershipServices entity)
        {
            _dbContext.Set<MembershipServices>().Update(entity);
        }

        public async Task UpdateAsync(Membership entity)
        {
            _dbContext.Set<Membership>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task Add(MembershipServices entity)
        {
            await _dbContext.Set<MembershipServices>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<MembershipServices?>> GetMembershipServicesByMembershipIdAsync(Guid membershipId)
        {
            return await _dbContext.MembershipServices
                .Where(ms => ms.MembershipId == membershipId && !ms.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<UserMembership>> GetUserMembershipsAsync(Guid userId)
        {
            return await _dbContext.UserMemberships
                .Include(um => um.Membership)
                .ThenInclude(m => m.MembershipServices)
                .Where(um => um.UserId == userId && !um.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<MembershipServices>> GetMembershipServicesByUserAsync(Guid userId)
        {
            var userMemberships = await GetUserMembershipsAsync(userId);

            return userMemberships
                .SelectMany(um => um.Membership.MembershipServices)
                .Where(ms => ms.MaxPostsAllowed > 0)
                .ToList();
        }
    }
}
