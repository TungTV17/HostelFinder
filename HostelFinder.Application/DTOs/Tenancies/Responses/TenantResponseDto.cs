using HostelFinder.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace HostelFinder.Application.DTOs.Tenancies.Responses;

public class TenantResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string? Description { get; set; }
    public string IdentityCardNumber { get; set; }
    public string? FrontImageUrl  { get; set; }
    public string? BackImageUrl { get; set; }
    public TemporaryResidenceStatus TemporaryResidenceStatus { get; set; }
}