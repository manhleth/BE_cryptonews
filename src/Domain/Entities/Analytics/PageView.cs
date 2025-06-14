namespace NewsPaper.src.Domain.Entities
{
    public class PageView : BaseEntity
    {
        public int PageViewId { get; set; }
        public int? UserId { get; set; } // Nullable cho anonymous users
        public int? NewsId { get; set; } // Nullable cho non-news pages
        public string PageUrl { get; set; }
        public string PageTitle { get; set; }
        public int SessionDuration { get; set; } // Seconds
        public DateTime ViewDate { get; set; } = DateTime.Now;
    }
}