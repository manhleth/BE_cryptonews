using NewsPaper.src.Application.DTOs;

namespace NewsPaper.src.Application.Interfaces
{
    public interface IBaseService<T> where T : class
    {
        Task<T> GetNewsByIdAsync(int id);
        Task<T> CreateNewsAsync(T newsDto);
        Task<T> UpdateNewsAsync(T newsDto, int id);
        Task<T> DeleteNewsAsync(int id);
        Task<List<T>> SearchNewsAsync();
    }
}
