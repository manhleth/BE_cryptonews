using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Infrastructure.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Infrastructure.Repositories
{
    public class PageViewRepository : BaseRepository<PageView>
    {
        public PageViewRepository(AppDbContext context) : base(context)
        {
        }
    }
}
