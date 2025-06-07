// src/Application/Services/NewsService.cs
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

        // SỬA PHƯƠNG THỨC NÀY
        public async Task<object> GetNewestAsycn()
        {
            try
            {
                var news = await _unitOfWork.News.GetTopNews(8);
                var listUserID = news.Select(x => x.UserId).Distinct().ToList();
                var listUser = await _unitOfWork.User.FindAsync(x => listUserID.Contains(x.UserId));

                List<NewsDto> listNewsDto = new List<NewsDto>();
                foreach (var item in news)
                {
                    var user = listUser.FirstOrDefault(x => x.UserId == item.UserId);
                    NewsDto newsDto = new NewsDto
                    {
                        NewsId = item.NewsId, // Thêm NewsId
                        Header = item.Header,
                        Title = item.Title,
                        Content = item.Content,
                        Footer = item.Footer,
                        TimeReading = item.TimeReading,
                        UserName = user?.Username ?? "Unknown",
                        avatar = user?.Avatar ?? "",
                        CategoryId = item.CategoryId,
                        ImagesLink = item.ImagesLink,
                        Links = item.Links,
                        UserId = item.UserId,
                        ChildrenCategoryId = item.ChildrenCategoryId,
                        CreatedDate = item.CreatedDate // Thêm CreatedDate
                    };
                    listNewsDto.Add(newsDto);
                }
                return listNewsDto;
            }
            catch (Exception ex)
            {
                return ex;
            }
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

        public async Task<object> UpdateNewsAsync(NewsDto newsDto)
        {
            try
            {
                var news = await _unitOfWork.News.GetByIdAsync(newsDto.NewsId);
                if (news == null)
                    throw new DirectoryNotFoundException($"Can't find news with id {newsDto.NewsId}");

                news.Header = newsDto.Header;
                news.Title = newsDto.Title;
                news.Content = newsDto.Content;
                news.Footer = newsDto.Footer;
                news.TimeReading = newsDto.TimeReading;
                news.ModifiedDate = DateTime.UtcNow;
                if(newsDto.Links != null)
                {
                    news.Links = newsDto.Links;
                }
                else
                {
                    news.Links = "";
                }    
                  
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
        // Lấy danh sách bài viết nổi bật
        public async Task<object> GetFeaturedNews()
        {
            try
            {
                var featuredNews = await _unitOfWork.News.FindAsync(x => x.IsFeatured == true);
                var orderedNews = featuredNews.OrderBy(x => x.FeaturedOrder ?? 999).Take(2).ToList();

                var listUserID = orderedNews.Select(x => x.UserId).Distinct().ToList();
                var listUser = await _unitOfWork.User.FindAsync(x => listUserID.Contains(x.UserId));

                List<NewsDto> listNewsDto = new List<NewsDto>();
                foreach (var item in orderedNews)
                {
                    var user = listUser.FirstOrDefault(x => x.UserId == item.UserId);
                    NewsDto newsDto = new NewsDto
                    {
                        NewsId = item.NewsId,
                        Header = item.Header,
                        Title = item.Title,
                        Content = item.Content,
                        Footer = item.Footer,
                        TimeReading = item.TimeReading,
                        UserName = user?.Username ?? "Unknown",
                        avatar = user?.Avatar ?? "",
                        CategoryId = item.CategoryId,
                        ImagesLink = item.ImagesLink,
                        Links = item.Links,
                        UserId = item.UserId,
                        ChildrenCategoryId = item.ChildrenCategoryId,
                        CreatedDate = item.CreatedDate
                    };
                    listNewsDto.Add(newsDto);
                }
                return listNewsDto;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        // Đặt bài viết là nổi bật
        public async Task<object> SetFeaturedNews(int newsId, bool isFeatured)
        {
            try
            {
                var news = await _unitOfWork.News.FindOnlyByCondition(x => x.NewsId == newsId);
                if (news == null)
                    return "Không tìm thấy bài viết";

                if (isFeatured)
                {
                    // Kiểm tra số lượng bài viết nổi bật hiện tại
                    var currentFeaturedCount = await _unitOfWork.News.FindAsync(x => x.IsFeatured == true);
                    if (currentFeaturedCount.Count() >=2)
                    {
                        return "Đã đạt giới hạn 2 bài viết nổi bật. Vui lòng bỏ chọn một bài viết khác trước.";
                    }

                    // Đặt thứ tự cho bài viết mới
                    news.FeaturedOrder = currentFeaturedCount.Count() + 1;
                }
                else
                {
                    news.FeaturedOrder = null;
                }

                news.IsFeatured = isFeatured;
                await _unitOfWork.News.UpdateAsync(news);
                await _unitOfWork.SaveChangesAsync();

                return "Cập nhật thành công";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Cập nhật thứ tự bài viết nổi bật
        public async Task<object> UpdateFeaturedOrder(List<int> newsIds)
        {
            try
            {
                // Reset tất cả bài viết nổi bật
                var allFeaturedNews = await _unitOfWork.News.FindAsync(x => x.IsFeatured == true);
                foreach (var news in allFeaturedNews)
                {
                    news.IsFeatured = false;
                    news.FeaturedOrder = null;
                    await _unitOfWork.News.UpdateAsync(news);
                }

                // Cập nhật lại thứ tự mới
                for (int i = 0; i < newsIds.Count && i < 2; i++)
                {
                    var news = await _unitOfWork.News.FindOnlyByCondition(x => x.NewsId == newsIds[i]);
                    if (news != null)
                    {
                        news.IsFeatured = true;
                        news.FeaturedOrder = i + 1;
                        await _unitOfWork.News.UpdateAsync(news);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return "Cập nhật thứ tự thành công";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}