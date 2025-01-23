using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.RentalContract.Request;

public class ContractExtensionRequest
{
    public Guid rentalContractId { get; set; }
    [DataType(DataType.Date)]
    public DateTime newEndDate { get; set; }
}