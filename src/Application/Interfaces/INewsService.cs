using NewsPaper.src.Application.DTOs;

namespace NewsPaper.src.Application.Interfaces
{
    public interface INewsService
    {
        Task<NewsDto> GetNewsByIdAsync(int id);
        Task<NewsDto> CreateNewsAsync(CreateNewsDto newsDto);
        Task<NewsDto> UpdateNewsAsync(NewsDto newsDto, int id);
        Task<NewsDto> DeleteNewsAsync(int id);
        Task<List<NewsDto>> SearchNewsAsync();
    }
}
