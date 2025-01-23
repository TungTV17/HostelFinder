using System.ComponentModel.DataAnnotations;
using HostelFinder.Domain.Enums;

namespace HostelFinder.Application.DTOs.ServiceCost.Request;

public class UpdateServiceCostDto
{
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "UnitCost must be non-negative.")]
    public decimal UnitCost { get; set; }

    [Required]
    public UnitType Unit { get; set; }
    [Required]
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}