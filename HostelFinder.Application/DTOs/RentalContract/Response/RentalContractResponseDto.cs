namespace HostelFinder.Application.DTOs.RentalContract.Response;

public class RentalContractResponseDto
{
    public Guid Id { get; set; }
    public string? TenantName { get; set; }

    public string? RoomName { get; set; }

    public DateTime StartDate { get; set; } 
    public DateTime? EndDate { get; set; } 

    public decimal MonthlyRent { get; set; } 

    public decimal DepositAmount { get; set; }
    
    public string? Status { get; set; }
}