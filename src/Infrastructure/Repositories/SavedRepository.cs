using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Infrastructure.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Infrastructure.Repositories
{
    public class SavedRepository : BaseRepository<Saved>
    {
        public SavedRepository(AppDbContext context) : base(context)
        {
        }
    }
}
