using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Interfaces;
using NewsPaper.src.Domain.Interfaces;
using AutoMapper;

namespace NewsPaper.src.Infrastructure.Services
{
    public class NewService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;


        public NewService(INewsRepository newsRepository, IMapper mapper)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
        }

        public Task<NewsDto> CreateNewsAsync(CreateNewsDto newsDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteNewsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<NewsDto> GetNewsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<NewsDto>> SearchNewsAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateNewsAsync(NewsDto newsDto)
        {
            throw new NotImplementedException();
        }
    }
}
