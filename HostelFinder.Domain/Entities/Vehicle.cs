using RoomFinder.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HostelFinder.Domain.Common;
using HostelFinder.Domain.Enums;

namespace HostelFinder.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string VehicleName { get; set; }

        [Required]
        [MaxLength(20)]
        public string LicensePlate { get; set; }

        [Required]
        [MaxLength(50)]
        public VehicleType VehicleType { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }



        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }
    }
}
