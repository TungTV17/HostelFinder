using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.RentalContract.Request
{
    public class AddRentalContractDto
    {
        public AddTenantDto? AddTenantDto { get; set; }

        public Guid RoomId { get;set; }

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal MonthlyRent { get; set; }

        public decimal DepositAmount { get; set; }
        public int PaymentCycleDays { get; set; }
        public string? ContractTerms { get; set; }

    }
}
