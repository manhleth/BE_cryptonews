namespace NewsPaper.src.Application.DTOs
{
    public class TrackActivityDto
    {
        public string ActivityType { get; set; } // LOGIN, VIEW_NEWS, COMMENT, SAVE_POST, REGISTER
        public int? RelatedNewsId { get; set; }
    }

    public class UserActivityResponseDto
    {
        public int UserActivityId { get; set; }
        public int UserId { get; set; }
        public string ActivityType { get; set; }
        public int? RelatedNewsId { get; set; }
        public DateTime ActivityDate { get; set; }
        public string TimeAgo { get; set; }
    }
}