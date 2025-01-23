using System.Linq.Expressions;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{

    public class RentalContractRepository : BaseGenericRepository<RentalContract>, IRentalContractRepository
    {
        public RentalContractRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }


        public async Task<RentalContract?> CheckExpiredContractAsync(Guid roomId, DateTime startDate, DateTime? endDate)
        {
            var roomExists = await _dbContext.RentalContracts
                        .AnyAsync(rt => rt.RoomId == roomId && !rt.IsDeleted);

            if (!roomExists)
            {
                return null;
            }
            var rentalContract = await _dbContext.RentalContracts
            .FirstOrDefaultAsync(rt => rt.RoomId == roomId
                                        && rt.EndDate.HasValue
                                        && (rt.StartDate <= endDate) && (rt.EndDate >= startDate)
                                        && !rt.IsDeleted
                                        );
            return rentalContract;
        }

        public async Task<(List<RentalContract> rentalContracts, int totalRecord)> GetAllMatchingRentalContractAysnc(Guid hostelId, string? searchPhrase,string? statusFilter, int pageNumber, int pageSize,
            string? sortBy, SortDirection sortDirection)
        {
            // lấy ra những phòng thuộc hostel
            var rooms =  _dbContext.Rooms
                .AsNoTracking()
                .Include(x => x.Invoices)
                .Where(r => r.HostelId == hostelId && !r.IsDeleted)
                .Select(r => r.Id);  
            var searchPhraseLower = searchPhrase?.ToLower();

            var query = _dbContext.RentalContracts
                .AsNoTracking()
                .Include(x => x.Room)
                .Include(x => x.Tenant)
                .Where(rt => rooms.Contains(rt.RoomId) && !rt.IsDeleted);
            if (!string.IsNullOrEmpty(searchPhrase))
            {
                query = query.Where(x => x.Room.RoomName.ToLower().Contains(searchPhraseLower)
                || x.Tenant.FullName.ToLower().Contains(searchPhraseLower));
            }
            if(statusFilter != null)
            {
                if (statusFilter == "Đã kết thúc")
                {
                    query = query.Where(x => x.EndDate.HasValue && x.EndDate.Value.Date < DateTime.Now.Date);
                }
                else if (statusFilter == "Trong thời hạn")
                {
                    query = query.Where(x => x.StartDate.Date <= DateTime.Now.Date && (x.EndDate == null || x.EndDate.Value.Date > DateTime.Now.Date));
                }
                else if(statusFilter == "Sắp kết thúc")
                {
                    query = query.Where(x => x.EndDate.HasValue && x.EndDate.Value.AddMonths(-1) <= DateTime.Now.Date && x.EndDate.Value.Date >= DateTime.Now.Date);
                }
                else if(statusFilter == "Chưa bắt đầu")
                {
                    query = query.Where(x => x.StartDate.Date > DateTime.Now.Date);
                }
            }
            var totalRecords = await query.CountAsync();
            
            if (!string.IsNullOrEmpty(sortBy))
            {
                var columnsSelector = new Dictionary<string, Expression<Func<RentalContract, object>>>
                {
                    { nameof(Room.RoomName), x => x.Room.RoomName},
                    { nameof(Tenant.FullName), x => x.Tenant.FullName},
                    { nameof(RentalContract.StartDate), x => x.StartDate},
                    { nameof(RentalContract.EndDate.HasValue), x => x.EndDate.Value},
                    { nameof(RentalContract.LastModifiedOn), x => x.LastModifiedOn},
                    { nameof(RentalContract.CreatedOn), x => x.CreatedOn},
                };

                if (columnsSelector.ContainsKey(sortBy))
                {
                    var selectedColumn = columnsSelector[sortBy];
                    query = sortDirection == SortDirection.Ascending
                        ? query.OrderBy(selectedColumn)
                        : query.OrderByDescending(selectedColumn);
                }
            }
            var rentalContracts = await query
                .OrderByDescending(x => x.CreatedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (rentalContracts: rentalContracts, totalRecord: totalRecords);
        }


        public async Task<RentalContract?> GetRoomRentalContrctByRoom(Guid roomId)
        {
            var currentDate = DateTime.Now;
            return await _dbContext.RentalContracts
                .Include(rt => rt.Room)
                .Include(rt => rt.Tenant)
                .Where(rt => rt.RoomId == roomId
                        && rt.StartDate.Date <= currentDate
                            && (rt.EndDate == null || rt.EndDate.Value >= currentDate)
                                && !rt.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<RentalContract> GetActiveRentalContractAsync(Guid roomId)
        {
            return await _dbContext.RentalContracts
                .AsNoTracking()
                .Where(rc => rc.RoomId == roomId)
                .FirstOrDefaultAsync();
        }

        public async Task<RentalContract> GetRentalContractByRoomIdAsync(Guid roomId)
        {
            return await _dbContext.RentalContracts
                .AsNoTracking()
                .FirstOrDefaultAsync(rc => rc.RoomId == roomId);
        }

    }
}
