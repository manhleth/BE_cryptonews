using NewsPaper.src.Domain.Entities;

namespace NewsPaper.src.Domain.Interfaces
{
    public interface INewsRepository
    {
        Task<News> GetByIdAsync(int id);
        Task  AddAsync(News news);
        Task UpdateAsync(News news);
        Task DeleteAsync(News news);
    }
}
