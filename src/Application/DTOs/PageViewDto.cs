namespace NewsPaper.src.Application.DTOs
{
    public class TrackPageViewDto
    {
        public int? NewsId { get; set; }
        public string PageUrl { get; set; }
        public string PageTitle { get; set; }
        public int SessionDuration { get; set; } = 0;
    }

    public class PageViewResponseDto
    {
        public int PageViewId { get; set; }
        public int? UserId { get; set; }
        public int? NewsId { get; set; }
        public string PageUrl { get; set; }
        public string PageTitle { get; set; }
        public int SessionDuration { get; set; }
        public DateTime ViewDate { get; set; }
    }
}