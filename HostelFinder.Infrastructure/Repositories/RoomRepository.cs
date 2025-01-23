using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.InkML;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Exceptions;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class RoomRepository : BaseGenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Room> GetRoomWithDetailsAndServiceCostsByIdAsync(Guid roomId)
        {
            var room = await _dbContext.Rooms
                    .Include(r => r.RoomDetails)
                    .Include(r => r.Hostel)
                    .ThenInclude(h => h.ServiceCosts)
                    .FirstOrDefaultAsync(r => r.Id == roomId && !r.IsDeleted);
            if (room == null)
            {
                throw new NotFoundException("Không tìm thấy phòng trọ nào!");
            }
            return room;
        }

        public async Task<List<Room>> ListAllWithDetailsAsync()
        {
            return await _dbContext.Rooms
                .Include(r => r.Hostel)
                          .ThenInclude(h => h.ServiceCosts)
                          .Include(r => r.RoomDetails)
                          .ToListAsync();
        }

        public async Task<List<Room>> GetRoomsByHostelIdAsync(Guid hostelId, int? floor)
        {
            IQueryable<Room> query = _dbContext.Rooms
                            .AsNoTracking()
                            .Include(r => r.Hostel)
                            .ThenInclude(h => h.ServiceCosts)
                                .ThenInclude(sc => sc.Service)
                            .Include(r => r.Invoices)
                            .Include(r => r.Images)
                            .Where(r => r.HostelId == hostelId && !r.IsDeleted);
            if (floor.HasValue)
            {
                query = query.Where(r => r.Floor == floor);
            }

            query = query.OrderByDescending(x => x.CreatedOn);
            return await query.ToListAsync();

        }

        public async Task<List<Room>> GetRoomsByHostelIdAsync(Guid hostelId)
        {
            IQueryable<Room> query = _dbContext.Rooms
                            .AsNoTracking()
                            .Include(r => r.Hostel)
                            .ThenInclude(h => h.ServiceCosts)
                                .ThenInclude(sc => sc.Service)
                            .Include(r => r.Invoices)
                            .Include(r => r.Images)
                            .Include(rt => rt.RoomTenancies)
                            .Where(r => r.HostelId == hostelId && !r.IsDeleted);
            return await query.ToListAsync();

        }

        public async Task<bool> RoomExistsAsync(string roomName, Guid hostelId)
        {
            return await _dbContext.Rooms.AnyAsync(r => r.RoomName == roomName && r.HostelId == hostelId && !r.IsDeleted);
        }

        public async Task<Room> GetRoomByIdAsync(Guid roomId)
        {
            var room = await _dbContext.Rooms
                .Include(r => r.Hostel)
                .ThenInclude(sc => sc.ServiceCosts)
                .Include(r => r.Invoices)
                .Include(r => r.Images)
                .Include(r => r.RoomAmenities)
                .FirstOrDefaultAsync(r => r.Id == roomId && !r.IsDeleted);

            if (room == null)
            {
                throw new NotFoundException("Không tìm thấy phòng trọ nào!");
            }

            return room;
        }

        public async Task<List<Room>> GetRoomsByHostelAsync(Guid hostelId)
        {
            var query = _dbContext.Rooms
                .AsNoTracking()
                .Include(r => r.Hostel)
                .ThenInclude(h => h.ServiceCosts)
                .ThenInclude(sc => sc.Service)
                .Include(r => r.Invoices)
                .Include(r => r.Images)
                .Where(r => r.HostelId == hostelId && !r.IsDeleted);
            return await query.ToListAsync();
        }

        public Task<(IEnumerable<Room> Data, int TotalRecords)> GetAllMatchingInLandLordAsync(Guid landlordId, string? searchPhrase, int pageSize, int pageNumber, string? sortBy,
            SortDirection? sortDirection)
        {
            throw new NotImplementedException();
        }
    }
}
