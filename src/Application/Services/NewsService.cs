using AutoMapper;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
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
            catch (Exception ex)
            {
                return ex;
            }
        }


        public async Task<object> GetNewsByChildrenCategoryID(int childrenCategoryID)
        {
            try
            {
                var news = await _unitOfWork.News.FindAsync(x => x.ChildrenCategoryId == childrenCategoryID);
                var listUserID = news.Select(x => x.UserId).Distinct().ToList();
                var s = listUserID.Count();
                var listUser = await _unitOfWork.User.FindAsync(x => listUserID.Contains(x.UserId));
                List<ListNewsDtoResponse> listNews = new List<ListNewsDtoResponse>();
                foreach (var item in news)
                {
                    ListNewsDtoResponse listNewsDtoResponse = new ListNewsDtoResponse();
                    listNewsDtoResponse.UserName = listUser.Where(x => x.UserId == item.UserId).Select(x => x.Username).FirstOrDefault();
                    listNewsDtoResponse.UserAvartar = listUser.Where(x => x.UserId == item.UserId).Select(x => x.Avatar).FirstOrDefault();
                    listNewsDtoResponse.Header = item.Header;
                    listNewsDtoResponse.Title = item.Title;
                    listNewsDtoResponse.Links = item.Links;
                    listNewsDtoResponse.TimeReading = item.TimeReading.ToString() + " minutes to read";
                    listNewsDtoResponse.ImagesLink = item.ImagesLink;
                    var hourago = (DateTime.Now.Hour - item.CreatedDate.Value.Hour);
                    var timeAgo = "";
                    if (hourago > 0)
                    {
                        timeAgo = hourago.ToString() + " Hour ago";
                    }
                    else
                    {
                        timeAgo = (DateTime.Now.Day - item.CreatedDate.Value.Day).ToString() + " Day ago";
                    }
                    listNewsDtoResponse.TimeAgo = timeAgo;
                    listNewsDtoResponse.NewsID = item.NewsId;
                    listNews.Add(listNewsDtoResponse);
                }
                return listNews;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public async Task<object> FindNewsByKeyWord(string keyword)
        {
            try
            {
                var news = await _unitOfWork.News.FindAsync(x => x.Title.Contains(keyword) || x.Header.Contains(keyword) || x.Content.Contains(keyword));
                return _mapper.Map<List<ListNewsDtoResponse>>(news);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }



        public async Task<object> GetNewsByCategoryTop(int category)
        {
            try
            {
                var news = await _unitOfWork.News.GetByConditionTop(x => x.CategoryId == category, 5);
                var listUserID = news.Select(x => x.UserId).Distinct().ToList();
                var s = listUserID.Count();
                var listUser = await _unitOfWork.User.FindAsync(x => listUserID.Contains(x.UserId));
                List<ListNewsDtoResponse> listNews = new List<ListNewsDtoResponse>();
                foreach (var item in news)
                {
                    ListNewsDtoResponse listNewsDtoResponse = new ListNewsDtoResponse();
                    listNewsDtoResponse.UserName = listUser.Where(x => x.UserId == item.UserId).Select(x => x.Username).FirstOrDefault();
                    listNewsDtoResponse.UserAvartar = listUser.Where(x => x.UserId == item.UserId).Select(x => x.Avatar).FirstOrDefault();
                    listNewsDtoResponse.Header = item.Header;
                    listNewsDtoResponse.Title = item.Title;
                    listNewsDtoResponse.Links = item.Links;
                    listNewsDtoResponse.TimeReading = item.TimeReading.ToString() + " minutes to read";
                    listNewsDtoResponse.ImagesLink = item.ImagesLink;
                    var hourago = (DateTime.Now.Hour - item.CreatedDate.Value.Hour);
                    var timeAgo = "";
                    if (hourago > 0)
                    {
                        timeAgo = hourago.ToString() + " Hour ago";
                    }
                    else
                    {
                        timeAgo = (DateTime.Now.Day - item.CreatedDate.Value.Day).ToString() + " Day ago";
                    }
                    listNewsDtoResponse.TimeAgo = timeAgo;
                    listNewsDtoResponse.NewsID = item.NewsId;
                    listNews.Add(listNewsDtoResponse);
                }
                return listNews;
            }
            catch (Exception ex)
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

        public async Task<object> DeleteNewsAsync(int UserID, int id)
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
            var user = await _unitOfWork.User.GetByIdAsync(news.UserId);
            var newsById = new NewsDto()
            {
                Header = news.Header,
                Title = news.Title,
                Content = news.Content,
                Footer = news.Footer,
                TimeReading = news.TimeReading,
                UserName = user.Username,
                CategoryId = news.CategoryId,
                avatar = user.Avatar,
                ImagesLink = news.ImagesLink
            };
            if (news == null)
                return $"Can't find news with id {id}";
            return newsById;
        }

        public async Task<object> GetYourCreatePost(int userID)
        {
            var news = await _unitOfWork.News.FindAsync(x => x.UserId == userID);
            return _mapper.Map<List<YourPostDto>>(news);
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
            catch (Exception ex)
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
