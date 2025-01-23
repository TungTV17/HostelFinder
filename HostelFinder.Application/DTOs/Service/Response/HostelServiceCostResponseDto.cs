namespace HostelFinder.Application.DTOs.Service.Response
{
    public class HostelServiceCostResponseDto
    {
        public decimal UnitCost { get; set; }

        public string? Unit { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}
