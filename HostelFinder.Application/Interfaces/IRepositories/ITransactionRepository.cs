using HostelFinder.Application.Common;
using HostelFinder.Domain.Entities;

namespace HostelFinder.Application.Interfaces.IRepositories
{
    public interface ITransactionRepository : IBaseGenericRepository<Transaction>
    {
        Task<Transaction?> GetByOrderCodeAsync(long orderCode);
    }
}
