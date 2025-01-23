using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class VehicleRepository : BaseGenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<Vehicle>> GetByTenantAsync(Guid tenantId)
        {
            return await _dbContext.Vehicles
                .Where(v => v.TenantId == tenantId && !v.IsDeleted)
                .ToListAsync();
        }
    }
}
