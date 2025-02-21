using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{

    public class NewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NewsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<NewsDto> CreateNewsAsync(CreateNewsDto newsDto)
        {
            var news = _mapper.Map<News>(newsDto);
            await _unitOfWork.News.AddAsync(news);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<NewsDto>(news);
        }

        public Task<NewsDto> CreateNewsAsync(NewsDto newsDto)
        {
            throw new NotImplementedException();
        }

        //public Task<NewsDto> CreateNewsAsync(NewsDto newsDto)
        //{
        //    //throw new NotImplementedException();
        //    var news = _mapper.Map<News>(newsDto);
        //    await _unitOfWork.News.AddAsync(news);
        //    await _unitOfWork.SaveChangesAsync();
        //    return _mapper.Map<NewsDto>(news);
        //}

        public async Task<NewsDto> DeleteNewsAsync(int id)
        {
            var news = await _unitOfWork.News.GetByIdAsync(id);
            if (news == null)
                throw new DirectoryNotFoundException($"Can't find news with id {id}");
            await _unitOfWork.News.DeleteAsync(news);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<NewsDto>(news);
        }

        public async Task<NewsDto> GetNewsByIdAsync(int id)
        {
            var news = await _unitOfWork.News.GetByIdAsync(id);
            if (news == null)
                throw new DirectoryNotFoundException($"Can't find news with id {id}");
            return _mapper.Map<NewsDto>(news);
        }

        public Task<List<NewsDto>> SearchNewsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<NewsDto> UpdateNewsAsync(NewsDto newsDto, int id)
        {
            var news = await _unitOfWork.News.GetByIdAsync(id);
            if (news == null)
                throw new DirectoryNotFoundException($"Can't find news with id {id}");

            news.Header = newsDto.Header;
            news.Title = newsDto.Title;
            news.Content = newsDto.Content;
            news.Footer = newsDto.Footer;
            news.TimeReading = newsDto.TimeReading;
            news.ModifiedDate = DateTime.UtcNow;

            await _unitOfWork.News.UpdateAsync(news);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<NewsDto>(news);
        }
    }
}
