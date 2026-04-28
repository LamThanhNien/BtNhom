using System.Linq.Expressions;

namespace ASC.DataAccess.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAllAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<IEnumerable<T>> FindAllByQuery(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> FindAllInAuditByQuery(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(string id);
    }
}
