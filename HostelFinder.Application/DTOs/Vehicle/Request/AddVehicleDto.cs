using HostelFinder.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HostelFinder.Application.DTOs.Vehicle.Request
{
    public class AddVehicleDto
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
        public IFormFile Image { get; set; }

        public Guid TenantId { get; set; }
    }
}
