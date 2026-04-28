using ASC.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ASC.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        private IDbContextTransaction? _transaction;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<T>(_dbContext);
            }
            return (IRepository<T>)_repositories[type];
        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction not started.");

            try
            {
                var result = await _dbContext.SaveChangesAsync();
                await _transaction.CommitAsync();
                return result;
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}