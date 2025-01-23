using HostelFinder.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; } 
        [Required]
        public long OrderCode { get; set; } 
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; } 
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "PENDING";
        public virtual User User { get; set; }
    }
}
