using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Infrastructure.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Infrastructure.Repositories
{
    public class DailyStatsRepository : BaseRepository<DailyStats>
    {
        public DailyStatsRepository(AppDbContext context) : base(context)
        {
        }
    }
}