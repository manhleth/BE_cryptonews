using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class CommentService 
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public Task<CommentDto> CreateNewsAsync(CommentDto newsDto)
        {
            throw new NotImplementedException();
        }

        public Task<CommentDto> DeleteNewsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CommentDto> GetNewsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<CommentDto>> SearchNewsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CommentDto> UpdateNewsAsync(CommentDto newsDto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
