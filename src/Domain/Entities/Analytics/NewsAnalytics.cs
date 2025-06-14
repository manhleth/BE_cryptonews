namespace NewsPaper.src.Domain.Entities
{
    public class NewsAnalytics : BaseEntity
    {
        public int NewsAnalyticsId { get; set; }
        public int NewsId { get; set; }
        public int ViewCount { get; set; } = 0;
        public int UniqueViewCount { get; set; } = 0;
        public int SaveCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;
        public decimal AverageReadTime { get; set; } = 0; // Seconds
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}