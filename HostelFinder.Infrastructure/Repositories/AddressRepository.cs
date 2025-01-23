using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories;

public class AddressRepository : BaseGenericRepository<Address>, IAddressRepository
{
    public AddressRepository(HostelFinderDbContext dbContext) : base(dbContext)
    {
    }
    public async Task<Address> GetAddressByHostelIdAsync(Guid hostelId)
    {
        return await _dbContext.Addresses
            .FirstOrDefaultAsync(a => a.HostelId == hostelId && !a.IsDeleted);
    }

}
