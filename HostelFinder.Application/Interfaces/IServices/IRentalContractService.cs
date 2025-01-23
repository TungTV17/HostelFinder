using HostelFinder.Application.DTOs.RentalContract.Request;
using HostelFinder.Application.DTOs.RentalContract.Response;
using HostelFinder.Application.DTOs.Room.Responses;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Common.Constants;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IRentalContractService
    {
        Task<Response<string>> CreateRentalContractAsync(AddRentalContractDto request);

        Task<RoomContractHistoryResponseDto> GetRoomContractHistoryLasest(Guid roomId);

        Task<Response<string>> TerminationOfContract(Guid rentalContractId);
        Task<Response<string>> ContractExtension(Guid rentalContractId, DateTime newEndDate);
        
        Task<PagedResponse<List<RentalContractResponseDto>>> GetRentalContractsByHostelIdAsync(Guid hostelId, string? searchPhrase,string statusFilter, int? pageNumber, int? pageSize, string? sortBy, SortDirection sortDirection);
        
        //Kiểm tra phòng ở hiện tại có hợp đồng nào hay không
        Task<bool> CheckContractExistAsync(Guid roomId);
    }
}
