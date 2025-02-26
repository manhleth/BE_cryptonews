using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Infrastructure.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Infrastructure.Repositories
{
    public class CommentRepository : BaseRepository<Comment>
    {
        public CommentRepository(AppDbContext context) : base(context)
        {
        }
    }
}
