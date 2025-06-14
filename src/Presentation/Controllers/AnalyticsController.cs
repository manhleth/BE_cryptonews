// src/Presentation/Controllers/AnalyticsController.cs
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Features;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : BaseController<AnalyticsController>
    {
        private readonly AnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AnalyticsController(
            AnalyticsService analyticsService,
            ILogger<AnalyticsController> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _analyticsService = analyticsService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Track page view - Can be used by both authenticated and anonymous users
        /// </summary>
        /// <param name="dto">Page view tracking data</param>
        /// <returns>Page view result</returns>
        [HttpPost("TrackPageView")]
        [AllowAnonymous]
        public async Task<ResponseData> TrackPageView([FromBody] TrackPageViewDto dto)
        {
            try
            {
                // Try to get user ID if authenticated, otherwise null for anonymous
                int? userId = null;
                try
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        userId = UserIDLogined;
                    }
                }
                catch
                {
                    // If not authenticated, userId remains null
                }

                var result = await _analyticsService.TrackPageView(dto, userId);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking page view");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Track user activity - Requires authentication
        /// </summary>
        /// <param name="dto">Activity tracking data</param>
        /// <returns>Activity result</returns>
        [HttpPost("TrackActivity")]
        public async Task<ResponseData> TrackActivity([FromBody] TrackActivityDto dto)
        {
            try
            {
                var result = await _analyticsService.TrackUserActivity(dto, UserIDLogined);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking user activity");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Get dashboard statistics - Admin only
        /// </summary>
        /// <param name="days">Number of days to include in statistics (default: 30)</param>
        /// <returns>Dashboard statistics</returns>
        [HttpGet("GetDashboardStats")]
        [Authorize(Roles = "1")]
        public async Task<ResponseData> GetDashboardStats(int days = 30)
        {
            try
            {
                var result = await _analyticsService.GetDashboardStats(days);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Get analytics for specific news article
        /// </summary>
        /// <param name="newsId">News article ID</param>
        /// <returns>News analytics</returns>
        [HttpGet("GetNewsAnalytics")]
        public async Task<ResponseData> GetNewsAnalytics(int newsId)
        {
            try
            {
                var result = await _analyticsService.GetNewsAnalytics(newsId);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news analytics");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Get popular content based on various metrics
        /// </summary>
        /// <param name="request">Filter and sorting options</param>
        /// <returns>List of popular content</returns>
        [HttpPost("GetPopularContent")]
        [AllowAnonymous]
        public async Task<ResponseData> GetPopularContent([FromBody] PopularContentRequestDto request)
        {
            try
            {
                var result = await _analyticsService.GetPopularNews(request);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular content");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Get popular content with GET method for easier frontend integration
        /// </summary>
        /// <param name="limit">Number of items to return</param>
        /// <param name="sortBy">Sort criteria (ViewCount, CommentCount, SaveCount)</param>
        /// <param name="categoryId">Filter by category (optional)</param>
        /// <returns>List of popular content</returns>
        [HttpGet("GetPopularContent")]
        [AllowAnonymous]
        public async Task<ResponseData> GetPopularContentGet(
            int limit = 10,
            string sortBy = "ViewCount",
            int? categoryId = null)
        {
            try
            {
                var request = new PopularContentRequestDto
                {
                    Limit = limit,
                    SortBy = sortBy,
                    CategoryId = categoryId,
                    FromDate = DateTime.Now.AddDays(-30), // Last 30 days by default
                    ToDate = DateTime.Now
                };

                var result = await _analyticsService.GetPopularNews(request);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular content");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Quick method to track news view (simplified)
        /// </summary>
        /// <param name="newsId">News ID</param>
        /// <param name="sessionDuration">Time spent reading (seconds)</param>
        /// <returns>Tracking result</returns>
        [HttpPost("TrackNewsView")]
        [AllowAnonymous]
        public async Task<ResponseData> TrackNewsView(int newsId, int sessionDuration = 0)
        {
            try
            {
                // Get news info for tracking
                var news = await _unitOfWork.News.GetByIdAsync(newsId);
                if (news == null)
                {
                    return new ResponseData { Data = "News not found", StatusCode = -1 };
                }

                var dto = new TrackPageViewDto
                {
                    NewsId = newsId,
                    PageUrl = $"/news/{newsId}",
                    PageTitle = news.Title,
                    SessionDuration = sessionDuration
                };

                // Try to get user ID if authenticated
                int? userId = null;
                try
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        userId = UserIDLogined;
                    }
                }
                catch
                {
                    // Anonymous user
                }

                var result = await _analyticsService.TrackPageView(dto, userId);

                // Also track VIEW_NEWS activity if user is authenticated
                if (userId.HasValue)
                {
                    var activityDto = new TrackActivityDto
                    {
                        ActivityType = "VIEW_NEWS",
                        RelatedNewsId = newsId
                    };
                    await _analyticsService.TrackUserActivity(activityDto, userId.Value);
                }

                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking news view");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Get trending topics based on recent activity
        /// </summary>
        /// <param name="hours">Time window in hours (default: 24)</param>
        /// <param name="limit">Number of trending topics to return</param>
        /// <returns>List of trending news</returns>
        [HttpGet("GetTrendingTopics")]
        [AllowAnonymous]
        public async Task<ResponseData> GetTrendingTopics(int hours = 24, int limit = 5)
        {
            try
            {
                var fromDate = DateTime.Now.AddHours(-hours);

                // Get recent page views for news
                var recentViews = await _unitOfWork.PageView.FindAsync(x =>
                    x.ViewDate >= fromDate && x.NewsId.HasValue);

                // Group by news and count views
                var trendingNews = recentViews
                    .GroupBy(x => x.NewsId.Value)
                    .OrderByDescending(g => g.Count())
                    .Take(limit)
                    .Select(g => new { NewsId = g.Key, ViewCount = g.Count() })
                    .ToList();

                // Get news details
                var newsIds = trendingNews.Select(x => x.NewsId).ToList();
                var news = await _unitOfWork.News.FindAsync(x => newsIds.Contains(x.NewsId));
                var users = await _unitOfWork.User.GetAllObject();

                var result = trendingNews.Select(trending =>
                {
                    var newsItem = news.FirstOrDefault(n => n.NewsId == trending.NewsId);
                    var author = users.FirstOrDefault(u => u.UserId == newsItem?.UserId);

                    return new
                    {
                        NewsId = trending.NewsId,
                        Title = newsItem?.Title ?? "",
                        Header = newsItem?.Header ?? "",
                        ImagesLink = newsItem?.ImagesLink ?? "",
                        ViewCount = trending.ViewCount,
                        AuthorName = author?.Username ?? "",
                        CreatedDate = newsItem?.CreatedDate ?? DateTime.MinValue,
                        TrendingScore = trending.ViewCount // Could be more complex formula
                    };
                }).ToList();

                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trending topics");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Get user activity history - User can see their own, Admin can see any user's
        /// </summary>
        /// <param name="userId">User ID (optional for admin)</param>
        /// <param name="limit">Number of activities to return</param>
        /// <returns>User activity history</returns>
        [HttpGet("GetUserActivityHistory")]
        public async Task<ResponseData> GetUserActivityHistory(int? userId = null, int limit = 50)
        {
            try
            {
                // Determine which user's activities to get
                int targetUserId;
                if (userId.HasValue)
                {
                    // Check if current user is admin or requesting their own data
                    if (User.IsInRole("1") || userId.Value == UserIDLogined)
                    {
                        targetUserId = userId.Value;
                    }
                    else
                    {
                        return new ResponseData { Data = "Unauthorized to view this user's activities", StatusCode = -1 };
                    }
                }
                else
                {
                    targetUserId = UserIDLogined;
                }

                var activities = await _unitOfWork.UserActivity.FindAsync(x => x.UserId == targetUserId);
                var recentActivities = activities
                    .OrderByDescending(x => x.ActivityDate)
                    .Take(limit)
                    .ToList();

                var result = recentActivities.Select(a => new UserActivityResponseDto
                {
                    UserActivityId = a.UserActivityId,
                    UserId = a.UserId,
                    ActivityType = a.ActivityType,
                    RelatedNewsId = a.RelatedNewsId,
                    ActivityDate = a.ActivityDate,
                    TimeAgo = CalculateTimeAgo(a.ActivityDate)
                }).ToList();

                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user activity history");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        // Helper method
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