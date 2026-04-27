using ASC.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ASC.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<T>> FindAllAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking().Where(predicate);
            if (orderBy is not null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllByQuery(Expression<Func<T, bool>> filter)
        {
            var result = await _dbSet.AsNoTracking().Where(filter).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<T>> FindAllInAuditByQuery(Expression<Func<T, bool>> filter)
        {
            var result = await _dbSet.AsNoTracking().Where(filter).ToListAsync();
            return result;
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await Task.FromResult(entity);
        }
    }
}
