using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class RoomAmentityRepository : BaseGenericRepository<RoomAmenities>, IRoomAmentityRepository
    {
        public RoomAmentityRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task DeleteByRoomIdAsync(Guid roomId)
        {
            var amenitiesToDelete = await _dbContext.RoomAmenities
                .Where(x => x.RoomId == roomId && !x.IsDeleted)
                .ToListAsync();
            if (amenitiesToDelete.Any())
            {
                foreach (var amenity in amenitiesToDelete)
                {
                     _dbContext.RoomAmenities.Remove(amenity);
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task AddRangeAsync(List<RoomAmenities> amenitiesList)
        {
            await _dbContext.RoomAmenities.AddRangeAsync(amenitiesList);
            await _dbContext.SaveChangesAsync();
        }

        public Task<List<RoomAmenities>> GetAmenitiesByRoomIdAsync(Guid roomId)
        {
            return _dbContext.RoomAmenities
                .Where(x => x.RoomId == roomId && !x.IsDeleted)
                .ToListAsync();
        }
    }
}
