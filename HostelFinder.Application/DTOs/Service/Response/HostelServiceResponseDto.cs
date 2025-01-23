using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.Service.Response
{
    public class HostelServiceResponseDto
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }

        public Guid HostelId { get; set; }

        public string? ServiceName { get; set; }
        public ChargingMethod ChargingMethod { get; set; }

        public string? HostelName {  get; set; }

        public ICollection<HostelServiceCostResponseDto >? ServiceCost { get; set; }

    }
}
