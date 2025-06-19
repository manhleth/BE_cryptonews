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
            if (string.IsNullOrEmpty(newsDto.Footer) || newsDto.Footer == "null")
            {
                news.Footer = null;
            }
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

        // FIXED: GetNewsByIdAsync - Sửa lại toàn bộ logic
        public async Task<object> GetNewsByIdAsync(int id)
        {
            try
            {
                // Kiểm tra news trước khi sử dụng
                var news = await _unitOfWork.News.GetByIdAsync(id);
                if (news == null)
                    return $"Can't find news with id {id}";

                // Lấy thông tin user
                var user = await _unitOfWork.User.GetByIdAsync(news.UserId);
                if (user == null)
                    return $"Can't find user for news {id}";

                // Lấy thông tin category nếu có
                string categoryName = "";
                if (news.CategoryId.HasValue)
                {
                    var category = await _unitOfWork.Category.GetByIdAsync(news.CategoryId.Value);
                    categoryName = category?.CategoryName ?? "";
                }

                // Tạo DTO đầy đủ thông tin
                var newsById = new NewsDto()
                {
                    NewsId = news.NewsId, // FIXED: Thêm NewsId
                    Header = news.Header,
                    Title = news.Title,
                    Content = news.Content,
                    Footer = news.Footer,
                    TimeReading = news.TimeReading,
                    UserName = user.Username,
                    CategoryId = news.CategoryId,
                    ChildrenCategoryId = news.ChildrenCategoryId, // FIXED: Thêm ChildrenCategoryId
                    avatar = user.Avatar,
                    ImagesLink = news.ImagesLink,
                    Links = news.Links, // FIXED: Thêm Links
                    UserId = news.UserId,
                    CreatedDate = news.CreatedDate // FIXED: Thêm CreatedDate
                };

                return newsById;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<object> GetYourCreatePost(int userID)
        {
            var news = await _unitOfWork.News.FindAsync(x => x.UserId == userID);
            return _mapper.Map<List<YourPostDto>>(news);
        }

        // FIXED: UpdateNewsAsync - Sửa lại toàn bộ logic
        public async Task<object> UpdateNewsAsync(NewsDto newsDto)
        {
            try
            {
                var news = await _unitOfWork.News.GetByIdAsync(newsDto.NewsId);
                if (news == null)
                    return $"Can't find news with id {newsDto.NewsId}";

                news.Header = newsDto.Header ?? news.Header;
                news.Title = newsDto.Title ?? news.Title;
                news.Content = newsDto.Content ?? news.Content;

                // Xử lý Footer đặc biệt
                if (newsDto.Footer == null || newsDto.Footer == "null" || string.IsNullOrEmpty(newsDto.Footer))
                {
                    news.Footer = null;
                }
                else
                {
                    news.Footer = newsDto.Footer;
                }

                news.TimeReading = newsDto.TimeReading ?? news.TimeReading;
                news.Links = newsDto.Links ?? "";
                news.ModifiedDate = DateTime.Now;

                if (newsDto.CategoryId.HasValue)
                {
                    news.CategoryId = newsDto.CategoryId.Value;
                }

                if (newsDto.ChildrenCategoryId.HasValue)
                {
                    news.ChildrenCategoryId = newsDto.ChildrenCategoryId.Value;
                }

                if (!string.IsNullOrEmpty(newsDto.ImagesLink))
                {
                    news.ImagesLink = newsDto.ImagesLink;
                }

                await _unitOfWork.News.UpdateAsync(news);
                await _unitOfWork.SaveChangesAsync();

                return new NewsDto
                {
                    NewsId = news.NewsId,
                    Header = news.Header,
                    Title = news.Title,
                    Content = news.Content,
                    Footer = news.Footer,
                    TimeReading = news.TimeReading,
                    Links = news.Links,
                    CategoryId = news.CategoryId,
                    ChildrenCategoryId = news.ChildrenCategoryId,
                    ImagesLink = news.ImagesLink,
                    UserId = news.UserId,
                    CreatedDate = news.CreatedDate
                };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // FIXED: Thêm method UpdateNewsFromForm để xử lý FormData
        public async Task<object> UpdateNewsFromForm(int newsId, string header, string title, string content,
            string footer, int? timeReading, string links, int? categoryId, int? childrenCategoryId, string imagesLink)
        {
            try
            {
                var news = await _unitOfWork.News.GetByIdAsync(newsId);
                if (news == null)
                    return $"Can't find news with id {newsId}";

                // Cập nhật tất cả fields
                news.Header = header ?? news.Header;
                news.Title = title ?? news.Title;
                news.Content = content ?? news.Content;
                news.Footer = footer ?? news.Footer;
                news.TimeReading = timeReading ?? news.TimeReading;
                news.Links = links ?? "";
                news.ModifiedDate = DateTime.Now;

                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    news.CategoryId = categoryId.Value;
                }

                if (childrenCategoryId.HasValue && childrenCategoryId.Value > 0)
                {
                    news.ChildrenCategoryId = childrenCategoryId.Value;
                }

                if (!string.IsNullOrEmpty(imagesLink))
                {
                    news.ImagesLink = imagesLink;
                }

                await _unitOfWork.News.UpdateAsync(news);
                await _unitOfWork.SaveChangesAsync();

                return "News updated successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<object> GetAllNews()
        {
            try
            {
                var newsList = await _unitOfWork.News.GetAllObject();
                var listUserID = newsList.Select(x => x.UserId).Distinct().ToList();
                var listUser = await _unitOfWork.User.FindAsync(x => listUserID.Contains(x.UserId));
                var listCategoryID = newsList.Where(x => x.CategoryId.HasValue).Select(x => x.CategoryId.Value).Distinct().ToList();
                var listCategory = await _unitOfWork.Category.FindAsync(x => listCategoryID.Contains(x.CategoryId));

                List<object> result = new List<object>();
                foreach (var item in newsList.OrderByDescending(x => x.CreatedDate))
                {
                    var user = listUser.FirstOrDefault(x => x.UserId == item.UserId);
                    var category = listCategory.FirstOrDefault(x => x.CategoryId == item.CategoryId);

                    result.Add(new
                    {
                        newsId = item.NewsId,
                        header = item.Header,
                        title = item.Title,
                        content = item.Content,
                        footer = item.Footer,
                        timeReading = item.TimeReading,
                        links = item.Links,
                        imagesLink = item.ImagesLink,
                        categoryId = item.CategoryId,
                        categoryName = category?.CategoryName ?? "",
                        childrenCategoryId = item.ChildrenCategoryId,
                        userId = item.UserId,
                        userName = user?.Username ?? "Unknown",
                        createdDate = item.CreatedDate,
                        modifiedDate = item.ModifiedDate,
                        isFeatured = item.IsFeatured,
                        featuredOrder = item.FeaturedOrder
                    });
                }

                return result;
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
                    if (currentFeaturedCount.Count() >= 2)
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