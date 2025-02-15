using Microsoft.EntityFrameworkCore;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;
using NewsPaper.src.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace NewsPaper.src.Infrastructure.Interfaces
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T news)
        {
            _context.Set<T>().Add(news);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task UpdateAsync(T news)
        {
            _context.Set<T>().Update(news);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T news)
        {
            _context.Set<T>().Remove(news);
            await _context.SaveChangesAsync();
        }
    }
}
