// src/Infrastructure/Persistence/UnitOfWork.cs
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
        private IDbContextTransaction _dbTransaction; // Đổi tên biến này
        private bool _disposed;

        // repository field
        private NewsRepository _news;
        private UserRepository _user;
        private SavedRepository _saved;
        private CategoryRepository _category;
        private CommentRepository _comment;
        private ChildrenCategoryRepository _childrenCategory;
        private WatchlistRepository _watchlist;
        private TransactionRepository _transactionRepo; // Đổi tên biến này

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // Lazy loading cho các repositories
        public NewsRepository News => _news ??= new NewsRepository(_context);
        public UserRepository User => _user ??= new UserRepository(_context);
        public SavedRepository Saved => _saved ??= new SavedRepository(_context);
        public CategoryRepository Category => _category ??= new CategoryRepository(_context);
        public CommentRepository Comment => _comment ??= new CommentRepository(_context);
        public ChildrenCategoryRepository ChildrenCategory => _childrenCategory ??= new ChildrenCategoryRepository(_context);
        public WatchlistRepository Watchlist => _watchlist ??= new WatchlistRepository(_context);
        public TransactionRepository Transaction => _transactionRepo ??= new TransactionRepository(_context);

        // transaction management
        public async Task BeginTransactionAsync()
        {
            _dbTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _dbTransaction?.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _dbTransaction?.Dispose();
                _dbTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_dbTransaction != null)
            {
                await _dbTransaction.RollbackAsync();
                _dbTransaction.Dispose();
                _dbTransaction = null;
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
                _dbTransaction?.Dispose();
            }
            _disposed = true;
        }
    }
}