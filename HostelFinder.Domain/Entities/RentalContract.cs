using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HostelFinder.Domain.Common;

namespace HostelFinder.Domain.Entities
{
    public class RentalContract : BaseEntity
    {
        [ForeignKey("Tenant")]
        [Required]
        public Guid TenantId { get; set; }

        [ForeignKey("Room")]
        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public DateTime StartDate { get; set; } 
        public DateTime? EndDate { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyRent { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DepositAmount { get; set; }

        [Required]
        public int PaymentCycleDays { get; set; } 

        public string? ContractTerms { get; set; } 

        // Navigation properties
        public virtual Tenant Tenant { get; set; }
        public virtual Room Room { get; set; }
    }
}
