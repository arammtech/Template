using Template.Domain.Common.IRepository;
using Template.Domain.Global;

namespace Template.Domain.Common.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        Result SaveChanges();
        Task<Result> SaveChangesAsync();

        Result StartTransaction();
        Task<Result> StartTransactionAsync();

        Result Commit();
        Task<Result> CommitAsync();

        Result Rollback();
        Task<Result> RollbackAsync();
    }
}
