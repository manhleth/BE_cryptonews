using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Infrastructure.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Infrastructure.Repositories
{
    public class NewsRepository : BaseRepository<News>
    {
        public NewsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
