using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Infrastructure.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Infrastructure.Repositories
{
    public class WatchlistRepository : BaseRepository<Watchlist>
    {
        public WatchlistRepository(AppDbContext context) : base(context)
        {
        }
    }
}