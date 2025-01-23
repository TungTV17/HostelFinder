using HostelFinder.Application.DTOs.Tenancies.Responses;
using HostelFinder.Application.Helpers;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Wrappers;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class TenantRepository : BaseGenericRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Tenant> GetByIdentityCardNumber(string identityCardNumber)
        {
            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(x => x.IdentityCardNumber == identityCardNumber && !x.IsDeleted);
            return tenant;
        }

        public async Task<PagedResponse<List<InformationTenanciesResponseDto>>> GetTenantsByHostelAsync(
          Guid hostelId,
          string? roomName,
          int pageNumber,
          int pageSize,
          string? status 
        )
        {
            var query = _dbContext.RoomTenancies
                .AsNoTracking()
                .Include(rt => rt.Room)
                .Include(rt => rt.Tenant)
                .Where(rt => rt.Room.HostelId == hostelId);

            // Nếu có tên phòng, lọc theo tên phòng
            if (!string.IsNullOrEmpty(roomName))
            {
                query = query.Where(rt => rt.Room.RoomName.Contains(roomName));
            }

            // Lọc theo status
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Đã rời phòng")
                {
                    query = query.Where(rt => rt.MoveOutDate.HasValue && rt.MoveOutDate < DateTime.Now);
                }
                else if (status == "Đang thuê")
                {
                    query = query.Where(rt => !rt.MoveOutDate.HasValue || rt.MoveOutDate >= DateTime.Now);
                }
            }

            // Sắp xếp theo ngày tạo
            query = query.OrderBy(rt => rt.CreatedOn);

            // Lấy dữ liệu
            var tenants = await query
                .Select(rt => new InformationTenanciesResponseDto
                {
                    TenancyId = rt.TenantId,
                    HostelId = hostelId,
                    RoomId = rt.RoomId,
                    RoomName = rt.Room.RoomName,
                    AvatarUrl = rt.Tenant.AvatarUrl,
                    FullName = rt.Tenant.FullName,
                    Email = rt.Tenant.Email,
                    Phone = rt.Tenant.Phone,
                    MoveInDate = rt.MoveInDate.ToString("dd/MM/yyyy"),
                    MoveOutDate = rt.MoveOutDate.HasValue ? rt.MoveOutDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "N/A",
                    // Trạng thái sẽ được tính trong Select
                    Status = rt.MoveOutDate.HasValue && rt.MoveOutDate < DateTime.Now.AddHours(7) ? "Đã rời phòng" : "Đang thuê"
                })
                .ToListAsync();
        
            // Tính tổng số bản ghi và phân trang
            var totalRecords = tenants.Count();
            var pagedData = tenants.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var pagedResponse = PaginationHelper.CreatePagedResponse(pagedData, pageNumber, pageSize, totalRecords);

            return pagedResponse;
        }

    }
}
