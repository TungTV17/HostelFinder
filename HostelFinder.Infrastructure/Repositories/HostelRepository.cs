using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace HostelFinder.Infrastructure.Repositories;

public class HostelRepository : BaseGenericRepository<Hostel>, IHostelRepository
{
    public HostelRepository(HostelFinderDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> CheckDuplicateHostelAsync(string hostelName, string province, string district,
         string commune, string detailAddress)
    {
        var duplicateHostel = await _dbContext.Hostels
            .Where(h => h.HostelName == hostelName
                && h.Address.Province == province
                && h.Address.District == district
                && h.Address.Commune == commune
                && h.Address.DetailAddress == detailAddress
                && !h.IsDeleted)
            .FirstOrDefaultAsync();

        if (duplicateHostel != null)
        {
            return true;
        }
        return false;
    }


    public async Task<IEnumerable<Hostel>> GetHostelsByUserIdAsync(Guid landlordId)
    {
        return await _dbContext.Hostels.Where(h => h.LandlordId == landlordId && !h.IsDeleted)
            .Include(a => a.Address)
            .Include(i => i.Images)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Hostel> Data, int TotalRecords)> GetAllMatchingAsync(string? searchPhrase,
        int pageSize, int pageNumber, string? sortBy, SortDirection sortDirection)
    {
        var searchPhraseLower = searchPhrase?.ToLower();
        var baseQuery = _dbContext.Hostels.Include(h => h.Landlord)
            .Where(x => searchPhraseLower == null || (x.HostelName.ToLower().Contains(searchPhraseLower)
                                                      || x.Landlord.Username.ToLower().Contains(searchPhraseLower)
                                                      && !x.IsDeleted));

        var totalRecords = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<Hostel, object>>>
            {
                { nameof(Hostel.HostelName), x => x.HostelName }
            };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var hostels = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (Data: hostels, TotalRecords: totalRecords);
    }

    public Task<Hostel> GetHostelByIdAndUserIdAsync(Guid hostelId, Guid userId)
    {
        return Task.FromResult(_dbContext.Hostels.Include(h => h.Address)
            .FirstOrDefault(h => h.Id == hostelId && h.LandlordId == userId && !h.IsDeleted));
    }

    public async Task<Hostel> GetHostelWithReviewsByPostIdAsync(Guid postId)
    {
        var post = await _dbContext.Posts
            .Include(p => p.Hostel)
            .FirstOrDefaultAsync(p => p.Id == postId);

        return post?.Hostel;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task<(IEnumerable<Hostel> Data, int TotalRecords)> GetAllMatchingInLandLordAsync(Guid landlordId, string? searchPhrase, int? pageSize, int? pageNumber, string? sortBy,
        SortDirection? sortDirection)
    {
        if (pageNumber == null)
        {
            pageNumber = 1;
        }
        if (pageSize == null)
        {
            pageSize = 10;
        }

        if (sortDirection == null)
        {
            sortDirection = SortDirection.Ascending;
        }

        var searchPhraseLower = searchPhrase?.ToLower();
        var baseQuery = _dbContext.Hostels.Include(h => h.Landlord)
            .Include(h => h.Address)
            .Include(h => h.Images)
             .Where(x => x.LandlordId == landlordId);
        baseQuery = baseQuery.Where(x => !x.IsDeleted && searchPhraseLower == null || (x.HostelName.ToLower().Contains(searchPhraseLower)
                                                                      || x.Landlord.Username.ToLower().Contains(searchPhraseLower)));

        var totalRecords = await baseQuery.CountAsync();
        //default sort by create on 
        if (sortBy == null)
        {
            baseQuery = baseQuery.OrderByDescending(x => x.CreatedOn);
        }


        if (sortBy != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<Hostel, object>>>
            {
                { nameof(Hostel.HostelName), x => x.HostelName },
                { nameof(Hostel.Size), x => x.Size},
                { nameof(Hostel.NumberOfRooms), x => x.NumberOfRooms},
                { nameof(Hostel.CreatedOn), x => x.CreatedOn},
                { nameof(Hostel.LastModifiedOn), x => x.LastModifiedOn}
            };

            var selectedColumn = columnsSelector[sortBy];

            baseQuery = sortDirection == SortDirection.Ascending
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }


        var hostels = await baseQuery
            .Skip(pageSize.Value * (pageNumber.Value - 1))
            .Take(pageSize.Value)
            .ToListAsync();

        return (Data: hostels, TotalRecords: totalRecords);
    }

    public async Task<Hostel?> GetHostelByIdAsync(Guid hostelId)
    {
        return await _dbContext.Hostels
            .Where(x => x.IsDeleted == false)
            .Include(h => h.Address)
            .Include(i => i.Images)
            .Include(s => s.HostelServices)
            .ThenInclude(s => s.Services)
            .AsSplitQuery()
            .FirstOrDefaultAsync(h => h.Id == hostelId && !h.IsDeleted);
    }

    public async Task<int> GetHostelCountAsync(Guid landlordId)
    {
        return await _dbContext.Hostels.Where(h => h.LandlordId == landlordId && !h.IsDeleted).CountAsync();
    }

    public async Task<int> GetTenantCountAsync(Guid landlordId)
    {
        return await _dbContext.RoomTenancies
                             .Where(rt => rt.Room.Hostel.LandlordId == landlordId && (rt.MoveOutDate > DateTime.Now.AddHours(8) || rt.MoveOutDate == null) && !rt.IsDeleted)
                             .Select(rt => rt.TenantId)
                             .Distinct()
                             .CountAsync();
    }

    public async Task<int> GetRoomCountAsync(Guid landlordId)
    {
        return await _dbContext.Rooms.Where(r => r.Hostel.LandlordId == landlordId && !r.IsDeleted).CountAsync();
    }

    public async Task<int> GetOccupiedRoomCountAsync(Guid landlordId)
    {
        return await _dbContext.Rooms.Where(r => r.Hostel.LandlordId == landlordId && r.IsAvailable == false && !r.IsDeleted).CountAsync();
    }

    public async Task<int> GetAvailableRoomCountAsync(Guid landlordId)
    {
        return await _dbContext.Rooms.Where(r => r.Hostel.LandlordId == landlordId && r.IsAvailable == true && !r.IsDeleted).CountAsync();
    }

    public async Task<int> GetAllInvoicesCountAsync(Guid landlordId)
    {
        return await _dbContext.InVoices.Where(i => i.Room.Hostel.LandlordId == landlordId && !i.IsDeleted).CountAsync();
    }

    public async Task<int> GetUnpaidInvoicesCountAsync(Guid landlordId)
    {
        return await _dbContext.InVoices.Where(i => i.Room.Hostel.LandlordId == landlordId && !i.IsPaid && !i.IsDeleted).CountAsync();
    }

    public async Task<int> GetExpiringContractsCountAsync(Guid landlordId, DateTime currentDate)
    {
        var nextMonthStart = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(1); 
        var nextMonthEnd = nextMonthStart.AddMonths(1).AddDays(-1);

        return await _dbContext.RentalContracts
                             .Where(rc => rc.Room.Hostel.LandlordId == landlordId
                                         && rc.EndDate.HasValue  
                                         && rc.EndDate.Value >= nextMonthStart
                                         && rc.EndDate.Value <= nextMonthEnd
                                         && !rc.IsDeleted)  
                             .CountAsync();
    }

    public async Task<int> GetPostCountAsync(Guid landlordId)
    {
        var hostelIds = await _dbContext.Hostels.Where(x => x.LandlordId == landlordId && !x.IsDeleted).Select(x => x.Id).ToListAsync();
        return await _dbContext.Posts
            .Where(x => hostelIds.Contains(x.HostelId) && !x.IsDeleted)
            .CountAsync();
    }
}