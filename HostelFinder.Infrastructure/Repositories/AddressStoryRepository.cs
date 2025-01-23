using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class AddressStoryRepository : BaseGenericRepository<AddressStory>, IAddressStoryRepository
    {
        public AddressStoryRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<AddressStory> GetAddressByStoryIdAsync(Guid storyId)
        {
            return await _dbContext.AddressStories
                .Where(address => address.StoryId == storyId)
                .FirstOrDefaultAsync();
        }
    }
}
