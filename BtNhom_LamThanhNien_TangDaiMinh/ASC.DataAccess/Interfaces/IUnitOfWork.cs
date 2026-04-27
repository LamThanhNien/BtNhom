using ASC.DataAccess.Interfaces;

namespace ASC.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        Task<int> CommitAsync();
        Task<int> CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
