namespace NewsPaper.src.Domain.Entities
{
    public class DailyStats : BaseEntity
    {
        public int DailyStatsId { get; set; }
        public DateTime Date { get; set; }
        public int TotalPageViews { get; set; } = 0;
        public int UniqueVisitors { get; set; } = 0;
        public int NewUsers { get; set; } = 0;
        public int NewPosts { get; set; } = 0;
        public int NewComments { get; set; } = 0;
    }
}