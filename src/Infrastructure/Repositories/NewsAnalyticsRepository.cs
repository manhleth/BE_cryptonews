using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Infrastructure.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Infrastructure.Repositories
{
    public class NewsAnalyticsRepository : BaseRepository<NewsAnalytics>
    {
        public NewsAnalyticsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
