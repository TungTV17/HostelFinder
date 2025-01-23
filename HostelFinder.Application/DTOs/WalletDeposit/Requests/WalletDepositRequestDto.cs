using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.Payment.Requests
{
    public class WalletDepositRequestDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Deposit amount must be greater than 0.")]
        public decimal Amount { get; set; }
    }
}
