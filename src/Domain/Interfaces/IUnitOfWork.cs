namespace NewsPaper.src.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        INewsRepository News { get; }

        // transaction
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
