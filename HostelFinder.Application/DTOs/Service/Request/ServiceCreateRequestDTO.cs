using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.Service.Request
{
    public class ServiceCreateRequestDTO
    {
        public string ServiceName { get; set; }
        public Guid HostelId { get; set; }
        public ChargingMethod ChargingMethod { get; set; }
    }
}
