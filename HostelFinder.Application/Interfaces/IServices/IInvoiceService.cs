using HostelFinder.Application.DTOs.InVoice.Requests;
using HostelFinder.Application.DTOs.Invoice.Responses;
using HostelFinder.Application.DTOs.InVoice.Responses;
using HostelFinder.Application.DTOs.Room.Responses;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Common.Constants;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IInvoiceService
    {
        Task<Response<List<InvoiceResponseDto>>> GetAllAsync();
        Task<Response<InvoiceResponseDto>> GetByIdAsync(Guid id);
        Task<Response<InvoiceResponseDto>> CreateAsync(AddInVoiceRequestDto invoiceDto);
        Task<Response<InvoiceResponseDto>> UpdateAsync(Guid id, UpdateInvoiceRequestDto invoiceDto);
        Task<Response<bool>> DeleteAsync(Guid id);
        Task<Response<InvoiceResponseDto>> GenerateMonthlyInvoicesAsync(Guid roomId, int billingMonth, int billingYear);

        Task<RoomInvoiceHistoryDetailsResponseDto?> GetInvoiceDetailInRoomLastestAsyc(Guid roomId);
        
        Task<Response<Boolean>> CheckInvoiceExistAsync(Guid roomId, int billingMonth, int billingYear);
        
        Task<PagedResponse<List<ListInvoiceResponseDto>>> GetAllInvoicesByHostelIdAsync(Guid hostelId, string? searchPhrase, int? pageNumber, int? pageSize, string? sortBy, SortDirection sortDirection);
        
        Task<Response<InvoiceResponseDto>> GetDetailInvoiceByIdAsync(Guid id);

        Task<Response<string>> CollectMoneyInvoice (CollectMoneyInvoiceRequest request);
        
        Task<bool> CheckInvoiceNotPaidAsync(Guid roomId);
        
        Task<bool> SendEmailInvoiceToTenantAsync(Guid landlordId,Guid invoiceId);

    }
}
