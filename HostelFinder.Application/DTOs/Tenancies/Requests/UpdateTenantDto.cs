using HostelFinder.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace HostelFinder.Application.DTOs.Tenancies.Requests;

public class UpdateTenantDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public IFormFile? AvatarImage { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string? Description { get; set; }
    public string IdentityCardNumber { get; set; }
    public IFormFile? FrontImageImage { get; set; }
    public IFormFile? BackImageImage { get; set; }
    public TemporaryResidenceStatus TemporaryResidenceStatus { get; set; }
}