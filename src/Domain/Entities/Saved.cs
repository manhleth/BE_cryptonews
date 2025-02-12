namespace NewsPaper.src.Domain.Entities
{
    public class Saved : BaseEntity
    {
        public int SavedId { get; set; }
        public int UserId { get; set; }

        public int NewsId { get; set; }
    }
}
