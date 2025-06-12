using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Infrastructure.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Infrastructure.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>
    {
        public TransactionRepository(AppDbContext context) : base(context)
        {
        }
    }
}