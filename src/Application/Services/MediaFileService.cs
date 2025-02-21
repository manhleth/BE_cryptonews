using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class MediaFileService 
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
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
