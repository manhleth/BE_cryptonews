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


        public async Task<object> GetNewestAsycn()
        {
            var news = await _unitOfWork.News.GetTopNews(8);
            return _mapper.Map<List<NewsDto>>(news);
        }

        public async Task<object> GetNewsByCategory(int category)
        {
            try
            {
                var news = await _unitOfWork.News.FindAsync(x => x.CategoryId == category);
                return _mapper.Map<List<NewsDto>>(news);
            }
            catch(Exception ex)
            {
                return ex;
            }
        }

        public async Task<object> CreateNewsAsync(CreateNewsDto newsDto)
        {
            var news = _mapper.Map<News>(newsDto);
            await _unitOfWork.News.AddAsync(news);
            await _unitOfWork.SaveChangesAsync();
            return newsDto;
        }

        public async Task<object> DeleteNewsAsync(int UserID,int id)
        {
            var news = await _unitOfWork.News.FindOnlyByCondition(x => x.UserId == UserID && x.NewsId == id);
            if (news == null)
                return $"Can't not find news with id: {id}";
            await _unitOfWork.News.DeleteAsync(news);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<NewsDto>(news);
        }
        public async Task<object> DeleteNewsAsyncByAdmin(int id)
        {
            var news = await _unitOfWork.News.FindOnlyByCondition(x => x.NewsId == id);
            if (news == null)
                return $"Can't not find news with id: {id}";
            await _unitOfWork.News.DeleteAsync(news);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<NewsDto>(news);
        }

        public async Task<object> GetNewsByIdAsync(int id)
        {
            var news = await _unitOfWork.News.GetByIdAsync(id);
            if (news == null)
                return $"Can't find news with id {id}";
            return _mapper.Map<NewsDto>(news);
        }

        public async Task<object> UpdateNewsAsync(NewsDto newsDto, int id)
        {
            try
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
            catch(Exception ex)
            {
                return ex;
            }
        }

        public async Task<object> GetAllNews()
        {
            try
            {
                return await _unitOfWork.News.GetAllObject();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
