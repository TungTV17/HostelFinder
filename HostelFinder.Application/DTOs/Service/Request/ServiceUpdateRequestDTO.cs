using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.Service.Request
{
    public class ServiceUpdateRequestDTO
    {
        public string? ServiceName { get; set; }
        public ChargingMethod ChargingMethod { get; set; }
    }
}
