namespace NewsPaper.src.Application.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalNews { get; set; }
        public int TotalComments { get; set; }
        public int TotalPageViews { get; set; }

        // Daily stats for charts
        public List<DailyStatsDto> DailyStats { get; set; } = new();

        // Recent activities
        public List<UserActivityResponseDto> RecentActivities { get; set; } = new();

        // Popular content
        public List<PopularNewsDto> PopularNews { get; set; } = new();
    }

    public class DailyStatsDto
    {
        public DateTime Date { get; set; }
        public int TotalPageViews { get; set; }
        public int UniqueVisitors { get; set; }
        public int NewUsers { get; set; }
        public int NewPosts { get; set; }
        public int NewComments { get; set; }
    }

    public class NewsAnalyticsDto
    {
        public int NewsId { get; set; }
        public string NewsTitle { get; set; }
        public int ViewCount { get; set; }
        public int UniqueViewCount { get; set; }
        public int SaveCount { get; set; }
        public int CommentCount { get; set; }
        public decimal AverageReadTime { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class PopularNewsDto
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Header { get; set; }
        public string ImagesLink { get; set; }
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }
        public int SaveCount { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class PopularContentRequestDto
    {
        public int Limit { get; set; } = 10;
        public string SortBy { get; set; } = "ViewCount"; // ViewCount, CommentCount, SaveCount
        public int? CategoryId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}