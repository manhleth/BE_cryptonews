using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;

namespace NewsPaper.src.Infrastructure.Interfaces
{
    public class NewsRepository : INewsRepository
    {
        private readonly AppDbContext _context;
        public NewsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<News> GetByIdAsync(int id)
        {
            return await _context.News.FindAsync(id);
        }

        public async Task AddAsync(News news)
        {
            _context.News.Add(news);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(News news)
        {
            _context.News.Update(news);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(News news)
        {
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
        }
    }
}
