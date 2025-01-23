using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class UserMembershipRepository : BaseGenericRepository<UserMembership>, IUserMembershipRepository
    {
        public UserMembershipRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<UserMembership> GetUserMembershipByUserIdAsync(Guid userId)
        {
            return await _dbContext.UserMemberships
                .Include(um => um.Membership)
                .ThenInclude(m => m.MembershipServices)
                .FirstOrDefaultAsync(um => um.UserId == userId && !um.IsDeleted);
        } 
        public async Task<List<UserMembership>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.UserMemberships
                .Include(um => um.Membership)
                .ThenInclude(m => m.MembershipServices)
                .Where(um => um.UserId == userId && !um.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<UserMembership>> GetExpiredMembershipsAsync()
        {
            return await _dbContext.UserMemberships
                 .Where(um => um.ExpiryDate < DateTime.Now && !um.IsDeleted)
                 .ToListAsync();
        }

        public async Task<UserMembership> GetTrialMembershipByUserIdAsync(Guid userId)
        {
            return await _dbContext.UserMemberships
                .FirstOrDefaultAsync(um => um.UserId == userId && um.IsPaid);
        }

        public async Task<UserMembership> GetByUserIdAndMembershipIdAsync(Guid userId, Guid membershipId)
        {
            return await _dbContext.UserMemberships
                .FirstOrDefaultAsync(um => um.UserId == userId && um.MembershipId == membershipId && !um.IsDeleted);
        }

        public async Task<List<UserMembership>> GetActiveUserMembershipsByMembershipIdAsync(Guid membershipId)
        {
            return await _dbContext.UserMemberships
                .Where(um => um.MembershipId == membershipId && um.ExpiryDate > DateTime.Now && !um.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<UserMembership>> GetUserMembershipsAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.UserMemberships
                .Include(m => m.Membership)
                .Where(um => um.StartDate >= startDate && um.StartDate <= endDate)
                .ToListAsync();
        }

    }

}
