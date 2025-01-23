using HostelFinder.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace HostelFinder.Application.DTOs.RentalContract.Request
{
    public class AddTenantDto
    {
        public string FullName { get; set; }
        public IFormFile? AvatarImage { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Description { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? DetailAddress { get; set; }
        public string IdentityCardNumber { get; set; }
        public IFormFile? FrontImageImage { get; set; }
        public IFormFile? BackImageImage { get; set; }
        public TemporaryResidenceStatus TemporaryResidenceStatus { get; set; }
    }
}
