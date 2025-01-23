using HostelFinder.Application.Common;
using HostelFinder.Infrastructure.Context;
using HostelFinder.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using RoomFinder.Domain.Common;
using System.Linq.Expressions;
using HostelFinder.Domain.Common;

namespace HostelFinder.Infrastructure.Common
{

    public class BaseGenericRepository<T> : IBaseGenericRepository<T> where T : BaseEntity
    {
        public readonly HostelFinderDbContext _dbContext;

        public BaseGenericRepository
            (HostelFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<T> AddAsync(T entity)
        {
            try
            {
                await _dbContext.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("An error occurred while adding the entity.", ex);
            }
            return entity;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            try
            {
                return await _dbContext.Set<T>().CountAsync(expression);
            }
            catch (Exception ex) 
            {
                throw new RepositoryException("An error occurred while counting the entities.", ex);
            }

        }

        public async Task<T> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if(entity == null)
            {
                return null;
            }
            
            entity.IsDeleted = true;
            await UpdateAsync(entity);
            return entity;
        }

        public async Task<T> DeletePermanentAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }
            try
            {
                _dbContext.Set<T>().Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("An error occurred while delete the entities.", ex);
            }
            return entity;
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            var entity =  await _dbContext.Set<T>().FirstOrDefaultAsync(expression);
            if (entity == null)
            {
                return null;
            }
            return entity;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var entity = await _dbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (entity == null)
            {
                return null;
            }
            return entity;
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<IList<T>> ListAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbContext.Set<T>().Where(expression).ToListAsync();
        }

        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("An error occurred while update the entity.", ex);
            }
            return entity;
        }
    }
}
