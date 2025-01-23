using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.Vehicle.Responses
{
    public class VehicleResponseDto
    {
        public Guid Id { get; set; }

        public string VehicleName { get; set; }

        public string LicensePlate { get; set; }

        public VehicleType VehicleType { get; set; }

        public string? ImageUrl { get; set; }
        public Guid TenantId { get; set; }
    }
}
