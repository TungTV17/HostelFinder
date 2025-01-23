using System.Linq.Expressions;

namespace HostelFinder.Application.Common
{
    public interface IBaseGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);

        Task<List<T>> ListAllAsync();

        Task<IList<T>> ListAsync(Expression<Func<T, bool>> expression);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);

        Task<T> AddAsync(T entity);

        Task<T> UpdateAsync(T entity);

        Task<T> DeleteAsync(Guid id);

        Task<T> DeletePermanentAsync(Guid id);

        Task<int> CountAsync(Expression<Func<T, bool>> expression);
    }
}
