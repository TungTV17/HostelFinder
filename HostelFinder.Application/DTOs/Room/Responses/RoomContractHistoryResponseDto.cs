using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.Room.Responses
{
    public class RoomContractHistoryResponseDto
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public int PaymentCycleDays { get; set; }
        public string? ContractTerms { get; set; }


    }
}
