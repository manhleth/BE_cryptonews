using NewsPaper.src.Application.DTOs;

namespace NewsPaper.src.Application.Interfaces
{
    public interface INewsService
    {
        Task<NewsDto> GetNewsByIdAsync(int id);
        Task<NewsDto> CreateNewsAsync(CreateNewsDto newsDto);
        Task UpdateNewsAsync(NewsDto newsDto);
        Task DeleteNewsAsync(int id);
        Task<List<NewsDto>> SearchNewsAsync();
    }
}
