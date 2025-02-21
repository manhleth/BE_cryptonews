using Microsoft.EntityFrameworkCore.Storage;
using NewsPaper.src.Domain.Interfaces;
using NewsPaper.src.Infrastructure.Interfaces;
using NewsPaper.src.Infrastructure.Repositories;

namespace NewsPaper.src.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {

        // dependency injection
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed;

        // repository field
        private NewsRepository _news;
        private UserRepository _user;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        
        // Lazy loading cho các repositories
        public NewsRepository News => _news ??= new NewsRepository(_context);

        public UserRepository User => _user ??= new UserRepository(_context);

        // transaction management
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction?.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
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

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
                _transaction?.Dispose();
            }
            _disposed = true;
        }
    }
}
