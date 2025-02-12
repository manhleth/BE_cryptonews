namespace NewsPaper.src.Domain.Entities
{
    public abstract class BaseEntity
    {
        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
