namespace NewsPaper.src.Domain.Entities
{
    public class UserActivity : BaseEntity
    {
        public int UserActivityId { get; set; }
        public int UserId { get; set; }
        public string ActivityType { get; set; } // LOGIN, VIEW_NEWS, COMMENT, SAVE_POST, REGISTER
        public int? RelatedNewsId { get; set; } // Nullable
        public DateTime ActivityDate { get; set; } = DateTime.Now;
    }
}
