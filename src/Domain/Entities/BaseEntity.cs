namespace NewsPaper.src.Domain.Entities
{
    public abstract class BaseEntity
    {
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }
    }
}
