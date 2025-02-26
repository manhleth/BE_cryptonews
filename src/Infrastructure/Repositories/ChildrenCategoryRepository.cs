using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Infrastructure.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Infrastructure.Repositories
{
    public class ChildrenCategoryRepository : BaseRepository<ChildrenCategory>
    {
        public ChildrenCategoryRepository(AppDbContext context) : base(context)
        {

        }
    }
}
