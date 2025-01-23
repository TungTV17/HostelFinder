using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.MeterReading.Response;

public class ServiceCostReadingResponse
{
    public Guid ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public decimal UnitCost { get; set; }
    public int? PreviousReading { get; set; }
    public int? CurrentReading { get; set; }
    public UnitType Unit { get; set; }
    public ChargingMethod ChargingMethod { get; set; }
}