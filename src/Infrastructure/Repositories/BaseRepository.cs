﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<T> FindOnlyByCondition(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
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

        public async Task<IEnumerable<T>> GetAllObject()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetTopNews(int top)
        {
            return await _context.Set<T>().OrderByDescending(x => EF.Property<DateTime>(x, "CreatedDate")).Take(top).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetByConditionTop(Expression<Func<T, bool>> predicate,int top)
        {
            return await _context.Set<T>().Where(predicate).OrderByDescending(x => EF.Property<DateTime>(x, "CreatedDate")).Take(top).ToListAsync();
        }
        public Task<IEnumerable<T>> GetAll(T news)
        {
            throw new NotImplementedException();
        }
    }
}
