using NewsPaper.src.Domain.Entities;

namespace NewsPaper.src.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T news);
        Task UpdateAsync(T news);
        Task DeleteAsync(T news);
    }
}
