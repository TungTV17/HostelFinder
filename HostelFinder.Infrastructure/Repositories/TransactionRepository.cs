using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HostelFinder.Infrastructure.Repositories
{
    public class TransactionRepository : BaseGenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Transaction?> GetByOrderCodeAsync(long orderCode)
        {
            return await _dbContext.Set<Transaction>().FirstOrDefaultAsync(t => t.OrderCode == orderCode);
        }
    }
}
