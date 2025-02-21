using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class SavedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public Task<SavedDto> CreateNewsAsync(SavedDto newsDto)
        {
            throw new NotImplementedException();
        }
        public Task<SavedDto> DeleteNewsAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<SavedDto> GetNewsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<List<SavedDto>> SearchNewsAsync()
        {
            throw new NotImplementedException();
        }
        public Task<SavedDto> UpdateNewsAsync(SavedDto newsDto, int id)
        {
            throw new NotImplementedException();
        }
    }
    
}
