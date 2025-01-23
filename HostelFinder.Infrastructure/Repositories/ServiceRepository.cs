using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Domain.Exceptions;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class ServiceRepository : BaseGenericRepository<Service>, IServiceRepository
    {
        public ServiceRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Service?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Services.FindAsync(id);
        }

        public async Task<bool> CheckDuplicateServiceAsync(string serviceName)
        {
            return await _dbContext.Services.AnyAsync(s => s.ServiceName == serviceName && !s.IsDeleted);
        }

        public async Task<IEnumerable<Service>> GetAllServiceAsync()
        {
            return await _dbContext.Services
                            .Include(s => s.ServiceCosts)
                            .ToListAsync(); 
        }

        public async Task<Service> GetServiceByIdAsync(Guid serviceId)
        {
            var service = await _dbContext.Services
                .Include(s => s.ServiceCosts)
                .FirstOrDefaultAsync(s => s.Id == serviceId && !s.IsDeleted);
            if (service == null)
            {
                throw new NotFoundException("Không tìm thấy dịch vụ nào!");
            }
            return service;
        }

        public async Task<IEnumerable<HostelServices>> GetServiceByHostelIdAsync(Guid hostelId)
        {
            var hostelServices = await _dbContext.HostelServices
                .Where(hs => hs.HostelId == hostelId && !hs.IsDeleted)
                .Include(hs => hs.Services)
                    .ThenInclude(s => s.ServiceCosts)
                .Include(hs => hs.Hostel)
                .ToListAsync();
            if (!hostelServices.Any())
            {
                throw new NotFoundException("Không tìm thấy dịch vụ nào trong nhà trọ");
            }

            return hostelServices;
        }

        public async Task<ServiceCost> GetCurrentServiceCostAsync(Guid hostelId, Guid serviceId)
        {
            return await _dbContext.ServiceCosts
                .FirstOrDefaultAsync(sc => sc.HostelId == hostelId
                && sc.ServiceId == serviceId
                    && sc.EffectiveFrom <= DateTime.Now
                        && (sc.EffectiveTo == null || sc.EffectiveTo >= sc.EffectiveFrom)
                            && !sc.IsDeleted);

        }

    }
}
