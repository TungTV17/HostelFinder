using HostelFinder.Application.Common;
using HostelFinder.Application.DTOs.Tenancies.Responses;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface ITenantRepository : IBaseGenericRepository<Tenant>
    {
        Task<Tenant> GetByIdentityCardNumber(string identityCardNumber);
        Task<PagedResponse<List<InformationTenanciesResponseDto>>> GetTenantsByHostelAsync(Guid hostelId, string? roomName, int pageNumber, int pageSize, string? status);
    }
}
