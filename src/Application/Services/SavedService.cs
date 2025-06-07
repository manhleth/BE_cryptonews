using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class SavedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly NewsService _newsService;

        public SavedService(IUnitOfWork unitOfWork, IMapper mapper, NewsService newsServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _newsService = newsServices;
        }

        public async Task<object> AddOrRemoveSaved(int newsID, int UserID)
        {
            var checkNews = await _newsService.GetNewsByIdAsync(newsID);
            if (checkNews == null)
            {
                return "can't find post";
            }
            var findSavedPost = await _unitOfWork.Saved.FindOnlyByCondition(x => x.UserId == UserID && x.NewsId == newsID);
            if (findSavedPost == null)
            {
                SavedDto savedDto = new SavedDto
                {
                    NewsID = newsID,
                    UserID = UserID,
                    Status = 1
                };
                var newSavedObject = _mapper.Map<Saved>(savedDto);
                await _unitOfWork.Saved.AddAsync(newSavedObject);
                await _unitOfWork.SaveChangesAsync();
                return newSavedObject;
            }
            else
            {
                if (findSavedPost.Status == 1)
                {
                    findSavedPost.Status = 0;
                    await _unitOfWork.SaveChangesAsync();
                    return "Remove saved post status successfully";
                }
                else
                {
                    findSavedPost.Status = 1;
                    await _unitOfWork.SaveChangesAsync();
                    return "Add saved post status successfully";
                }
            }
        }

        public async Task<object> GetListSavedByUser(int userId)
        {
            return await _unitOfWork.Saved.FindAsync(x => x.UserId == userId && x.Status == 1);
        }

        // Method cũ - vẫn giữ để backward compatibility
        public async Task<object> GetListSavedPostByUser(int userId, int categoryID)
        {
            var listsaved = await _unitOfWork.Saved.FindAsync(x => x.UserId == userId && x.Status == 1);
            var listNewsID = listsaved.Select(x => x.NewsId).ToList();
            var news = await _unitOfWork.News.FindAsync(x => listNewsID.Contains(x.NewsId) && x.CategoryId == categoryID);
            List<ListNewsDtoResponse> listNewsSaved = new List<ListNewsDtoResponse>();
            foreach (var item in news)
            {
                ListNewsDtoResponse items = new ListNewsDtoResponse();
                items.NewsID = item.NewsId;
                items.Header = item.Header;
                items.Title = item.Title;
                items.Links = item.Links;
                items.TimeReading = item.TimeReading.ToString() + " minutes to read";
                items.ImagesLink = item.ImagesLink;
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
                items.TimeAgo = timeAgo;
                var users = await _unitOfWork.User.FindOnlyByCondition(x => x.UserId == item.UserId);
                items.UserName = users.Username;
                items.UserAvartar = users.Avatar;
                listNewsSaved.Add(items);
            }
            return new
            {
                listSaved = listNewsSaved,
                total = news.Count()
            };
        }

        // NEW METHOD: Lấy tất cả bài viết đã lưu (category != 0)
        public async Task<object> GetAllSavedPostsByUser(int userId)
        {
            try
            {
                // Lấy danh sách bài viết đã lưu
                var listsaved = await _unitOfWork.Saved.FindAsync(x => x.UserId == userId && x.Status == 1);
                var listNewsID = listsaved.Select(x => x.NewsId).ToList();

                // Lấy tất cả bài viết có category != null và != 0
                var news = await _unitOfWork.News.FindAsync(x =>
                    listNewsID.Contains(x.NewsId) &&
                    x.CategoryId != null &&
                    x.CategoryId != 0
                );

                // Lấy thông tin user để hiển thị
                var userIds = news.Select(x => x.UserId).Distinct().ToList();
                var users = await _unitOfWork.User.FindAsync(x => userIds.Contains(x.UserId));

                // Lấy thông tin category để hiển thị
                var categoryIds = news.Where(x => x.CategoryId.HasValue)
                                     .Select(x => x.CategoryId.Value)
                                     .Distinct()
                                     .ToList();
                var categories = await _unitOfWork.Category.FindAsync(x => categoryIds.Contains(x.CategoryId));

                List<SavedPostDetailDto> listNewsSaved = new List<SavedPostDetailDto>();

                foreach (var item in news.OrderByDescending(x => x.CreatedDate))
                {
                    var user = users.FirstOrDefault(x => x.UserId == item.UserId);
                    var category = categories.FirstOrDefault(x => x.CategoryId == item.CategoryId);

                    SavedPostDetailDto savedPost = new SavedPostDetailDto
                    {
                        NewsID = item.NewsId,
                        Header = item.Header,
                        Title = item.Title,
                        Links = item.Links,
                        TimeReading = item.TimeReading?.ToString() + " minutes to read",
                        ImagesLink = item.ImagesLink,
                        UserName = user?.Username ?? "Unknown",
                        UserAvartar = user?.Avatar ?? "",
                        CategoryId = item.CategoryId,
                        CategoryName = category?.CategoryName ?? "Unknown Category",
                        ChildrenCategoryId = item.ChildrenCategoryId,
                        CreatedDate = item.CreatedDate,
                        TimeAgo = CalculateTimeAgo(item.CreatedDate)
                    };

                    listNewsSaved.Add(savedPost);
                }

                return new
                {
                    savedPosts = listNewsSaved,
                    total = listNewsSaved.Count,
                    categoriesCount = categoryIds.Count,
                    message = $"Found {listNewsSaved.Count} saved posts across {categoryIds.Count} categories"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    error = ex.Message,
                    savedPosts = new List<SavedPostDetailDto>(),
                    total = 0
                };
            }
        }

        // NEW METHOD: Lấy bài viết đã lưu theo category (cải tiến)
        public async Task<object> GetSavedPostsByCategory(int userId, int? categoryId = null)
        {
            try
            {
                var listsaved = await _unitOfWork.Saved.FindAsync(x => x.UserId == userId && x.Status == 1);
                var listNewsID = listsaved.Select(x => x.NewsId).ToList();

                IEnumerable<News> news;

                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    // Lấy bài viết theo category cụ thể
                    news = await _unitOfWork.News.FindAsync(x =>
                        listNewsID.Contains(x.NewsId) &&
                        x.CategoryId == categoryId.Value
                    );
                }
                else
                {
                    // Lấy tất cả bài viết có category != 0
                    news = await _unitOfWork.News.FindAsync(x =>
                        listNewsID.Contains(x.NewsId) &&
                        x.CategoryId != null &&
                        x.CategoryId != 0
                    );
                }

                var userIds = news.Select(x => x.UserId).Distinct().ToList();
                var users = await _unitOfWork.User.FindAsync(x => userIds.Contains(x.UserId));

                List<ListNewsDtoResponse> listNewsSaved = new List<ListNewsDtoResponse>();

                foreach (var item in news.OrderByDescending(x => x.CreatedDate))
                {
                    var user = users.FirstOrDefault(x => x.UserId == item.UserId);

                    ListNewsDtoResponse items = new ListNewsDtoResponse
                    {
                        NewsID = item.NewsId,
                        Header = item.Header,
                        Title = item.Title,
                        Links = item.Links,
                        TimeReading = item.TimeReading?.ToString() + " minutes to read",
                        ImagesLink = item.ImagesLink,
                        UserName = user?.Username ?? "Unknown",
                        UserAvartar = user?.Avatar ?? "",
                        TimeAgo = CalculateTimeAgo(item.CreatedDate)
                    };

                    listNewsSaved.Add(items);
                }

                return new
                {
                    listSaved = listNewsSaved,
                    total = news.Count(),
                    categoryFilter = categoryId,
                    message = categoryId.HasValue ?
                        $"Found {news.Count()} saved posts in category {categoryId}" :
                        $"Found {news.Count()} saved posts across all categories"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    error = ex.Message,
                    listSaved = new List<ListNewsDtoResponse>(),
                    total = 0
                };
            }
        }

        // NEW METHOD: Lấy thống kê bài viết đã lưu theo category
        public async Task<object> GetSavedPostsStatistics(int userId)
        {
            try
            {
                var listsaved = await _unitOfWork.Saved.FindAsync(x => x.UserId == userId && x.Status == 1);
                var listNewsID = listsaved.Select(x => x.NewsId).ToList();

                var news = await _unitOfWork.News.FindAsync(x =>
                    listNewsID.Contains(x.NewsId) &&
                    x.CategoryId != null &&
                    x.CategoryId != 0
                );

                var categoryIds = news.Where(x => x.CategoryId.HasValue)
                                     .Select(x => x.CategoryId.Value)
                                     .Distinct()
                                     .ToList();

                var categories = await _unitOfWork.Category.FindAsync(x => categoryIds.Contains(x.CategoryId));

                var statistics = news.GroupBy(x => x.CategoryId)
                                   .Select(g => new
                                   {
                                       CategoryId = g.Key,
                                       CategoryName = categories.FirstOrDefault(c => c.CategoryId == g.Key)?.CategoryName ?? "Unknown",
                                       Count = g.Count(),
                                       LatestSavedDate = g.Max(x => x.CreatedDate)
                                   })
                                   .OrderByDescending(x => x.Count)
                                   .ToList();

                return new
                {
                    totalSavedPosts = news.Count(),
                    categoriesWithSavedPosts = statistics.Count,
                    categoryStatistics = statistics,
                    lastSavedDate = news.Max(x => x.CreatedDate)
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    error = ex.Message,
                    totalSavedPosts = 0,
                    categoriesWithSavedPosts = 0,
                    categoryStatistics = new List<object>()
                };
            }
        }

        // Helper method để tính thời gian trước
        private string CalculateTimeAgo(DateTime? createdDate)
        {
            if (!createdDate.HasValue) return "Unknown time";

            var timeSpan = DateTime.Now - createdDate.Value;

            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            else if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            else if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} days ago";
            else if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} months ago";
            else
                return $"{(int)(timeSpan.TotalDays / 365)} years ago";
        }
    }

    // NEW DTO: DTO mới để hiển thị thông tin chi tiết hơn
    public class SavedPostDetailDto
    {
        public int NewsID { get; set; }
        public string Header { get; set; }
        public string Title { get; set; }
        public string Links { get; set; }
        public string TimeReading { get; set; }
        public string ImagesLink { get; set; }
        public string UserName { get; set; }
        public string UserAvartar { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ChildrenCategoryId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string TimeAgo { get; set; }
    }
}