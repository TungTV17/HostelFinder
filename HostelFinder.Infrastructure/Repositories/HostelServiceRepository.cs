using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class HostelServiceRepository : BaseGenericRepository<HostelServices>, IHostelServiceRepository
    {
        public HostelServiceRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<List<HostelServices>> GetServicesByHostelIdAsync(Guid hostelId)
        {
            return await _dbContext.HostelServices
                .Where(hs => hs.HostelId == hostelId && !hs.IsDeleted)
                .Include(hs => hs.Services) 
                .ToListAsync();
        }
    }
}
