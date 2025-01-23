using HostelFinder.Application.Common;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface IInVoiceRepository : IBaseGenericRepository<Invoice>
    {
        Task<IDbContextTransaction> BeginTransactionAsync();

        Task<Invoice?> GetLastInvoiceByIdAsync(Guid roomId);
        
        Task<List<Invoice>> GetInvoicesByRoomIdAsync(Guid roomId);
        
        Task<(List<Invoice> invoices, int  totalRecord)> GetAllMatchingInvoiceAysnc(Guid hostelId, string? searchPhrase, int pageNumber, int pageSize, string? sortBy, SortDirection sortDirection);
        
        Task<decimal> GetRoomRevenueByMonthAsync(Guid roomId, int month, int year);
        
        Task<decimal> GetRoomRevenueByYearAsync(Guid roomId, int year);
        
        Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId);
        
        Task<Invoice?> GetInvoiceByRoomIdAndMonthYearAsync(Guid roomId, int month, int year);

    }
}
