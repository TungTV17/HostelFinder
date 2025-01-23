using HostelFinder.Application.DTOs.Payment.Requests;
using HostelFinder.Application.DTOs.WalletDeposit.Responses;
using HostelFinder.Application.Wrappers;

namespace HostelFinder.Application.Interfaces.IServices
{
    public interface IWalletService
    {
        Task<Response<string>> CheckTransactionStatusAsync(long orderCode);
        Task<Response<DepositResponseDto>> DepositAsync(WalletDepositRequestDto depositRequest);
    }
}
