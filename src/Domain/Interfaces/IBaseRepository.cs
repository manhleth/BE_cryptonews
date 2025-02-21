using NewsPaper.src.Domain.Entities;
using System.Collections;

namespace NewsPaper.src.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T news);
        Task UpdateAsync(T news);
        Task DeleteAsync(T news);
        Task<IEnumerable<T>> GetAll(T news);
    }
}
