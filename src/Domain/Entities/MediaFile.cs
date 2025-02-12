namespace NewsPaper.src.Domain.Entities
{
    public class MediaFile : BaseEntity
    {
        public string MediaFileId { get; set; }

        public string ImageUrl { get; set; }    
        
        public string VideoUrl { get; set; }
    }
}
