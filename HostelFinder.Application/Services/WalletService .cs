using HostelFinder.Application.DTOs.Payment.Requests;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Net.payOS.Types;
using Net.payOS;
using HostelFinder.Application.Wrappers;
using HostelFinder.Application.DTOs.WalletDeposit.Responses;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly PayOS _payOS;
        private readonly IConfiguration _configuration;

        public WalletService(IUserRepository userRepository, IConfiguration configuration, ITransactionRepository transactionRepository, INotificationRepository notificationRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _configuration = configuration;
            _payOS = new PayOS(
                configuration["PayOS:ClientId"],
                configuration["PayOS:ApiKey"],
                configuration["PayOS:ChecksumKey"]);
            _notificationRepository = notificationRepository;
        }

        public async Task<Response<string>> CheckTransactionStatusAsync(long orderCode)
        {
            // Retrieve the transaction by orderCode
            var transaction = await _transactionRepository.GetByOrderCodeAsync(orderCode);
            if (transaction == null)
            {
                return new Response<string>("Transaction not found.");
            }

            // Check the payment status from PayOS
            var paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);

            if (paymentInfo.status == "PAID")
            {
                // Retrieve the user
                var user = await _userRepository.GetByIdAsync(transaction.UserId);
                if (user != null)
                {
                    // Update wallet balance and transaction status
                    user.WalletBalance += transaction.Amount;
                    transaction.Status = "Completed";

                    await _userRepository.UpdateAsync(user);
                    await _transactionRepository.UpdateAsync(transaction);
                    var notification = new Notification
                    {
                        Message = $"Bạn đã chuyển thành công {transaction.Amount} vào ví.",
                        UserId = user.Id,
                        CreatedOn = DateTime.Now
                    };
                    await _notificationRepository.AddAsync(notification);

                    return new Response<string>("Completed");
                }
            }
            else if (paymentInfo.status == "Failed")
            {
                // Update transaction status to failed
                transaction.Status = "Failed";
                await _transactionRepository.UpdateAsync(transaction);

                return new Response<string>("Failed");
            }

            return new Response<string>("Pending");
        }

        public async Task<Response<DepositResponseDto>> DepositAsync(WalletDepositRequestDto depositRequest)
        {
            // Check if user exists
            var user = await _userRepository.GetByIdAsync(depositRequest.UserId);
            if (user == null)
            {
                return new Response<DepositResponseDto>("User not found.");
            }

            // Generate a unique order code
            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Create a new transaction
            var transaction = new Domain.Entities.Transaction
            {
                UserId = user.Id,
                OrderCode = orderCode,
                Amount = depositRequest.Amount,
                Status = "Pending",
                CreatedOn = DateTime.UtcNow,
                CreatedBy = user.FullName
            };

            // Save transaction to the database
            await _transactionRepository.AddAsync(transaction);

            // Create payment data for deposit
            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)(depositRequest.Amount), // Convert amount to smallest unit
                description: $"Nạp tiền",
                items: new List<ItemData> { new ItemData("Wallet Deposit", 1, (int)(depositRequest.Amount)) },
                returnUrl: _configuration["PayOS:ReturnUrl"],
                cancelUrl: _configuration["PayOS:CancelUrl"]
            );

            try
            {
                // Generate payment link
                var createPaymentResult = await _payOS.createPaymentLink(paymentData);

                // Return payment URL and orderCode
                var response = new DepositResponseDto
                {
                    PaymentUrl = createPaymentResult.checkoutUrl,
                    OrderCode = orderCode
                };

                return new Response<DepositResponseDto>(response, "Payment URL generated successfully!");
            }
            catch (Exception ex)
            {
                return new Response<DepositResponseDto>($"Error generating deposit payment link: {ex.Message}");
            }
        }


    }
}
