using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Exceptions;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class RoomTenancyRepository : BaseGenericRepository<RoomTenancy>, IRoomTenancyRepository
    {
        public RoomTenancyRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> CountCurrentTenantsAsync(Guid roomId)
        {
            return await _dbContext.RoomTenancies
                .AsNoTracking()
                .Where(rt => rt.RoomId == roomId && (rt.MoveOutDate == null || rt.MoveOutDate.Value.Date > DateTime.Now.Date) && !rt.IsDeleted)
                .CountAsync();
        }
        // lấy ra danh sách người đang thuê trọ hiện tại
        public async Task<List<RoomTenancy>> GetRoomTenacyByIdAsync(Guid roomId)
        {
            return await _dbContext.RoomTenancies
                .Include(x => x.Tenant)
                .Include(x => x.Room)
                .Where(x => x.RoomId == roomId && x.MoveInDate <=DateTime.Now
                                    &&(x.MoveOutDate == null || x.MoveOutDate.Value > DateTime.Now.AddHours(8)) && !x.IsDeleted)
                .AsNoTracking()
                .OrderBy(x => x.MoveInDate)
                .ToListAsync();
            
        }

        public async Task<List<RoomTenancy>> GetTenacyCurrentlyByRoom(Guid roomId, DateTime startDate, DateTime? endDate)
        {
            var tanecyCurrentInRoomList =  await _dbContext.RoomTenancies
               .Include(x => x.Tenant)
               .Include(x => x.Room)
               .Where(x => x.RoomId == roomId 
                                    && endDate.HasValue 
                                        && x.MoveOutDate.HasValue 
                                            && startDate <= x.MoveInDate && x.MoveInDate <= endDate
                                                && !x.IsDeleted)
               .ToListAsync();
            if(tanecyCurrentInRoomList.Count == 0)
            {
                return null;
            }
            return tanecyCurrentInRoomList;
        }

        public async Task<RoomTenancy?> GetEarliestRoomTenancyByRoomIdAsync(Guid roomId)
        {
            return await _dbContext.RoomTenancies
                .AsNoTracking()
                .Where(rt => rt.RoomId == roomId)
                .OrderBy(rt => rt.CreatedOn) 
                .FirstOrDefaultAsync(); 
        }

        public Task<int> CountCurrentTenantsByRoomsInMonthAsync(Guid roomId, int month, int year)
        {
            var startDate = new DateTime(year, month, 1); 
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return _dbContext.RoomTenancies
                .AsNoTracking()
                .Where(rt => rt.RoomId == roomId 
                             && rt.MoveInDate <= endDate
                             && (rt.MoveOutDate == null || rt.MoveOutDate >= startDate) 
                             && !rt.IsDeleted)
                .CountAsync();        
        }

        public async Task<RoomTenancy?> GetRoomTenancyRepresentativeAsync(Guid roomId)
        {
            return await _dbContext.RoomTenancies
                .AsNoTracking()
                .Include(x => x.Tenant)
                .Where(x => x.RoomId == roomId && !x.IsDeleted)
                .OrderBy(x => x.CreatedOn)
                .FirstOrDefaultAsync();
        }

        public async Task<RoomTenancy?> GetRoomTenancyByTenantIdAsync(Guid tenantId)
        {
            return await _dbContext.RoomTenancies
                .AsNoTracking()
                .Include(x => x.Tenant)
                .Where(rt => rt.TenantId == tenantId)
                .FirstOrDefaultAsync(); 
        }

    }
}
