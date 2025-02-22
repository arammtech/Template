using Microsoft.EntityFrameworkCore.Storage;
using Template.Domain.Common.IRepository;
using Template.Domain.Common.IUnitOfWork;
using Template.Repository.EntityFrameworkCore.Context;
using Template.Repository.Repository;

namespace Template.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (!_repositories.TryGetValue(typeof(TEntity), out object? repository))
            {
                repository = new Repository<TEntity>(_context);
                _repositories.Add(typeof(TEntity), repository);
            }

            return (IRepository<TEntity>)repository;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void StartTransaction()
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _currentTransaction = _context.Database.BeginTransaction();
        }

        public async Task StartTransactionAsync()
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public void Commit()
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            _context.SaveChanges();
            _currentTransaction.Commit();
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }

        public async Task CommitAsync()
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            await _context.SaveChangesAsync();
            await _currentTransaction.CommitAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public void Rollback()
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No active transaction to roll back.");

            _currentTransaction.Rollback();
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }

        public async Task RollbackAsync()
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No active transaction to roll back.");

            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public void Dispose()
        {
            _context.Dispose();
            _currentTransaction?.Dispose();
        }
    }
}
