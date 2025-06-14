using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Application.Services
{
    public class AnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AnalyticsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Track page view
        public async Task<object> TrackPageView(TrackPageViewDto dto, int? userId = null)
        {
            try
            {
                var pageView = new PageView
                {
                    UserId = userId,
                    NewsId = dto.NewsId,
                    PageUrl = dto.PageUrl,
                    PageTitle = dto.PageTitle,
                    SessionDuration = dto.SessionDuration,
                    ViewDate = DateTime.Now
                };

                await _unitOfWork.PageView.AddAsync(pageView);
                await _unitOfWork.SaveChangesAsync();

                // Update daily stats
                await UpdateDailyStats(DateTime.Now.Date, "pageview", userId);

                // Update news analytics if it's a news page
                if (dto.NewsId.HasValue)
                {
                    await UpdateNewsAnalytics(dto.NewsId.Value, "view", userId, dto.SessionDuration);
                }

                return _mapper.Map<PageViewResponseDto>(pageView);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Track user activity
        public async Task<object> TrackUserActivity(TrackActivityDto dto, int userId)
        {
            try
            {
                var activity = new UserActivity
                {
                    UserId = userId,
                    ActivityType = dto.ActivityType,
                    RelatedNewsId = dto.RelatedNewsId,
                    ActivityDate = DateTime.Now
                };

                await _unitOfWork.UserActivity.AddAsync(activity);
                await _unitOfWork.SaveChangesAsync();

                // Update daily stats based on activity type
                if (dto.ActivityType == "REGISTER")
                {
                    await UpdateDailyStats(DateTime.Now.Date, "newuser", userId);
                }
                else if (dto.ActivityType == "COMMENT")
                {
                    await UpdateDailyStats(DateTime.Now.Date, "comment", userId);
                    if (dto.RelatedNewsId.HasValue)
                    {
                        await UpdateNewsAnalytics(dto.RelatedNewsId.Value, "comment", userId);
                    }
                }
                else if (dto.ActivityType == "SAVE_POST" && dto.RelatedNewsId.HasValue)
                {
                    await UpdateNewsAnalytics(dto.RelatedNewsId.Value, "save", userId);
                }

                var response = _mapper.Map<UserActivityResponseDto>(activity);
                response.TimeAgo = CalculateTimeAgo(activity.ActivityDate);
                return response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Get dashboard statistics
        public async Task<object> GetDashboardStats(int days = 30)
        {
            try
            {
                var fromDate = DateTime.Now.Date.AddDays(-days);

                // Total counts
                var totalUsers = (await _unitOfWork.User.GetAllObject()).Count();
                var totalNews = (await _unitOfWork.News.GetAllObject()).Count();
                var totalComments = (await _unitOfWork.Comment.GetAllObject()).Count();
                var totalPageViews = (await _unitOfWork.PageView.FindAsync(x => x.ViewDate >= fromDate)).Count();

                // Daily stats for charts
                var dailyStats = await GetDailyStatsForPeriod(fromDate, DateTime.Now.Date);

                // Recent activities (last 20)
                var recentActivities = await _unitOfWork.UserActivity.GetTopNews(20);
                var recentActivitiesDto = recentActivities.Select(a => new UserActivityResponseDto
                {
                    UserActivityId = a.UserActivityId,
                    UserId = a.UserId,
                    ActivityType = a.ActivityType,
                    RelatedNewsId = a.RelatedNewsId,
                    ActivityDate = a.ActivityDate,
                    TimeAgo = CalculateTimeAgo(a.ActivityDate)
                }).ToList();

                // Popular news (top 10 by views)
                var popularNews = await GetPopularNews(new PopularContentRequestDto { Limit = 10, SortBy = "ViewCount" });

                return new DashboardStatsDto
                {
                    TotalUsers = totalUsers,
                    TotalNews = totalNews,
                    TotalComments = totalComments,
                    TotalPageViews = totalPageViews,
                    DailyStats = dailyStats,
                    RecentActivities = recentActivitiesDto,
                    PopularNews = popularNews as List<PopularNewsDto> ?? new List<PopularNewsDto>()
                };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Get news analytics
        public async Task<object> GetNewsAnalytics(int newsId)
        {
            try
            {
                var newsAnalytics = await _unitOfWork.NewsAnalytics.FindOnlyByCondition(x => x.NewsId == newsId);
                if (newsAnalytics == null)
                {
                    // Create if doesn't exist
                    newsAnalytics = new NewsAnalytics { NewsId = newsId };
                    await _unitOfWork.NewsAnalytics.AddAsync(newsAnalytics);
                    await _unitOfWork.SaveChangesAsync();
                }

                var news = await _unitOfWork.News.GetByIdAsync(newsId);
                if (news == null) return "News not found";

                return new NewsAnalyticsDto
                {
                    NewsId = newsAnalytics.NewsId,
                    NewsTitle = news.Title,
                    ViewCount = newsAnalytics.ViewCount,
                    UniqueViewCount = newsAnalytics.UniqueViewCount,
                    SaveCount = newsAnalytics.SaveCount,
                    CommentCount = newsAnalytics.CommentCount,
                    AverageReadTime = newsAnalytics.AverageReadTime,
                    LastUpdated = newsAnalytics.LastUpdated
                };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Get popular content
        public async Task<List<PopularNewsDto>> GetPopularNews(PopularContentRequestDto request)
        {
            try
            {
                var newsAnalytics = await _unitOfWork.NewsAnalytics.GetAllObject();
                var allNews = await _unitOfWork.News.GetAllObject();
                var allUsers = await _unitOfWork.User.GetAllObject();

                var query = from analytics in newsAnalytics
                            join news in allNews on analytics.NewsId equals news.NewsId
                            join user in allUsers on news.UserId equals user.UserId
                            select new PopularNewsDto
                            {
                                NewsId = news.NewsId,
                                Title = news.Title,
                                Header = news.Header,
                                ImagesLink = news.ImagesLink,
                                ViewCount = analytics.ViewCount,
                                CommentCount = analytics.CommentCount,
                                SaveCount = analytics.SaveCount,
                                AuthorName = user.Username,
                                CreatedDate = news.CreatedDate ?? DateTime.MinValue
                            };

                // Apply filters
                if (request.CategoryId.HasValue)
                {
                    query = query.Where(x => allNews.Any(n => n.NewsId == x.NewsId && n.CategoryId == request.CategoryId.Value));
                }

                if (request.FromDate.HasValue)
                {
                    query = query.Where(x => x.CreatedDate >= request.FromDate.Value);
                }

                if (request.ToDate.HasValue)
                {
                    query = query.Where(x => x.CreatedDate <= request.ToDate.Value);
                }

                // Sort
                switch (request.SortBy?.ToLower())
                {
                    case "commentcount":
                        query = query.OrderByDescending(x => x.CommentCount);
                        break;
                    case "savecount":
                        query = query.OrderByDescending(x => x.SaveCount);
                        break;
                    default: // ViewCount
                        query = query.OrderByDescending(x => x.ViewCount);
                        break;
                }

                return query.Take(request.Limit).ToList();
            }
            catch (Exception ex)
            {
                return new List<PopularNewsDto>();
            }
        }

        // Private helper methods
        private async Task UpdateDailyStats(DateTime date, string action, int? userId = null)
        {
            try
            {
                var dailyStats = await _unitOfWork.DailyStats.FindOnlyByCondition(x => x.Date.Date == date.Date);
                if (dailyStats == null)
                {
                    dailyStats = new DailyStats { Date = date };
                    await _unitOfWork.DailyStats.AddAsync(dailyStats);
                }

                switch (action.ToLower())
                {
                    case "pageview":
                        dailyStats.TotalPageViews++;
                        if (userId.HasValue)
                        {
                            // Check if this user already visited today
                            var existingView = await _unitOfWork.PageView.FindOnlyByCondition(x =>
                                x.UserId == userId.Value && x.ViewDate.Date == date.Date);
                            if (existingView == null)
                            {
                                dailyStats.UniqueVisitors++;
                            }
                        }
                        break;
                    case "newuser":
                        dailyStats.NewUsers++;
                        break;
                    case "comment":
                        dailyStats.NewComments++;
                        break;
                    case "newpost":
                        dailyStats.NewPosts++;
                        break;
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't fail the main operation
                Console.WriteLine($"Error updating daily stats: {ex.Message}");
            }
        }

        private async Task UpdateNewsAnalytics(int newsId, string action, int? userId = null, int sessionDuration = 0)
        {
            try
            {
                var analytics = await _unitOfWork.NewsAnalytics.FindOnlyByCondition(x => x.NewsId == newsId);
                if (analytics == null)
                {
                    analytics = new NewsAnalytics { NewsId = newsId };
                    await _unitOfWork.NewsAnalytics.AddAsync(analytics);
                }

                switch (action.ToLower())
                {
                    case "view":
                        analytics.ViewCount++;
                        if (userId.HasValue)
                        {
                            // Check if this is a unique view
                            var existingView = await _unitOfWork.PageView.FindOnlyByCondition(x =>
                                x.UserId == userId.Value && x.NewsId == newsId);
                            if (existingView == null)
                            {
                                analytics.UniqueViewCount++;
                            }
                        }
                        // Update average read time
                        if (sessionDuration > 0)
                        {
                            analytics.AverageReadTime = ((analytics.AverageReadTime * (analytics.ViewCount - 1)) + sessionDuration) / analytics.ViewCount;
                        }
                        break;
                    case "save":
                        analytics.SaveCount++;
                        break;
                    case "comment":
                        analytics.CommentCount++;
                        break;
                }

                analytics.LastUpdated = DateTime.Now;
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating news analytics: {ex.Message}");
            }
        }

        private async Task<List<DailyStatsDto>> GetDailyStatsForPeriod(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var stats = await _unitOfWork.DailyStats.FindAsync(x => x.Date >= fromDate && x.Date <= toDate);
                return stats.OrderBy(x => x.Date).Select(s => new DailyStatsDto
                {
                    Date = s.Date,
                    TotalPageViews = s.TotalPageViews,
                    UniqueVisitors = s.UniqueVisitors,
                    NewUsers = s.NewUsers,
                    NewPosts = s.NewPosts,
                    NewComments = s.NewComments
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<DailyStatsDto>();
            }
        }

        private string CalculateTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Vừa xong";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} giờ trước";
            if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} ngày trước";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";
            return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
        }
    }
}