using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class MediaFileService : IBaseService<MediaFileDto>
    {
        public Task<MediaFileDto> CreateNewsAsync(MediaFileDto newsDto)
        {
            throw new NotImplementedException();
        }
        public Task<MediaFileDto> DeleteNewsAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<MediaFileDto> GetNewsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<List<MediaFileDto>> SearchNewsAsync()
        {
            throw new NotImplementedException();
        }
        public Task<MediaFileDto> UpdateNewsAsync(MediaFileDto newsDto, int id)
        {
            throw new NotImplementedException();
        }
    }
    
}
