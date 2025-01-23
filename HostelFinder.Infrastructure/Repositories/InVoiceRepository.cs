using System.Linq.Expressions;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HostelFinder.Infrastructure.Repositories
{
    public class InVoiceRepository : BaseGenericRepository<Invoice>, IInVoiceRepository
    {
    
        public InVoiceRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task<Invoice?> GetLastInvoiceByIdAsync(Guid roomId)
        {
            return await _dbContext.InVoices
                .Include(x => x.InvoiceDetails)
                .ThenInclude(details => details.Service)
                .Where(x => x.RoomId == roomId && !x.IsDeleted)
                .OrderByDescending(x => x.BillingYear)
                .ThenByDescending(x => x.BillingMonth)
                .FirstOrDefaultAsync();
        }

        public Task<List<Invoice>> GetInvoicesByRoomIdAsync(Guid roomId)
        {
            return _dbContext.InVoices
                .AsNoTracking()
                .Include(x => x.InvoiceDetails)
                .ThenInclude(details => details.Service)
                .Where(x => x.RoomId == roomId && !x.IsDeleted)
                .OrderByDescending(x => x.BillingYear)
                .ThenByDescending(x => x.BillingMonth)
                .ToListAsync();
        }

        public async Task<(List<Invoice> invoices, int  totalRecord)> GetAllMatchingInvoiceAysnc(Guid hostelId, string? searchPhrase, int pageNumber, int pageSize, string? sortBy,
            SortDirection sortDirection)
        {
            var roomsQuery = _dbContext.Rooms
                .AsNoTracking()
                .Include(x => x.Invoices)
                .Where(r => r.HostelId == hostelId && !r.IsDeleted)
                .Select(r => r.Id);  

            var searchPhraseLower = searchPhrase?.ToLower();

            var query = _dbContext.InVoices
                .AsNoTracking()
                .Include(x => x.Room)
                .Include(x => x.InvoiceDetails)
                .ThenInclude(details => details.Service)
                .Where(x => roomsQuery.Contains(x.RoomId) && !x.IsDeleted);

            if (!string.IsNullOrEmpty(searchPhrase))
            {
                query = query.Where(x => x.Room.RoomName.ToLower().Contains(searchPhraseLower));
            }

            var totalRecords = await query.CountAsync();

            if (!string.IsNullOrEmpty(sortBy))
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Invoice, object>>>
                {
                    { nameof(Room.RoomName), x => x.Room.RoomName},
                    { nameof(Invoice.BillingMonth), x => x.BillingMonth},
                    { nameof(Invoice.BillingYear), x => x.BillingYear},
                    { nameof(Invoice.TotalAmount), x => x.TotalAmount},
                    { nameof(Invoice.IsPaid), x => x.IsPaid},
                    { nameof(Invoice.CreatedOn), x => x.CreatedOn},
                };

                if (columnsSelector.ContainsKey(sortBy))
                {
                    var selectedColumn = columnsSelector[sortBy];
                    query = sortDirection == SortDirection.Ascending
                        ? query.OrderBy(selectedColumn)
                        : query.OrderByDescending(selectedColumn);
                }
            }

            var invoices = await query
                .OrderByDescending(x => x.CreatedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (invoices: invoices, totalRecord: totalRecords);
        }

        public async Task<decimal> GetRoomRevenueByMonthAsync(Guid roomId, int month, int year)
        {
            var invoices = _dbContext.InVoices
                .AsNoTracking()
                .Include(x => x.Room)
                .Where(invoice => !invoice.IsDeleted && invoice.RoomId == roomId && invoice.BillingMonth == month &&
                                  invoice.BillingYear == year && invoice.IsPaid == true && !invoice.IsDeleted);
            
            decimal totalRevenue = await invoices.SumAsync(invoice => invoice.TotalAmount);
            return totalRevenue;
        }

        public Task<decimal> GetRoomRevenueByYearAsync(Guid roomId, int year)
        {
            var invoices = _dbContext.InVoices
                .AsNoTracking()
                .Include(x => x.Room)
                .Where(invoice => !invoice.IsDeleted && invoice.RoomId == roomId && invoice.BillingYear == year && invoice.IsPaid == true && !invoice.IsDeleted);
            
            return invoices.SumAsync(invoice => invoice.TotalAmount);
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId)
        {
            return await _dbContext.InVoices
                .Include(x => x.Room)
                .Include(x => x.InvoiceDetails)
                .ThenInclude(details => details.Service)
                .FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted);
        }

        public Task<Invoice?> GetInvoiceByRoomIdAndMonthYearAsync(Guid roomId, int month, int year)
        {
            return _dbContext.InVoices
                .AsNoTracking()
                .Include(x => x.InvoiceDetails)
                .ThenInclude(details => details.Service)
                .FirstOrDefaultAsync(x => x.RoomId == roomId && x.BillingMonth == month && x.BillingYear == year && !x.IsDeleted);
        }
    }
}
