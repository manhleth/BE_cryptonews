namespace NewsPaper.src.Application.DTOs
{
    public class NewsDto
    {
        public int NewsId { get; set; } // Thêm trường này
        public string? Header { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Footer { get; set; }
        public int? TimeReading { get; set; }
        public string? UserName { get; set; }
        public string? avatar { get; set; }
        public int? CategoryId { get; set; }
        public string? ImagesLink { get; set; }
        public string? Links { get; set; }
        public int UserId { get; set; }
        public int? ChildrenCategoryId { get; set; }
        public DateTime? CreatedDate { get; set; } // Thêm trường này
    }
}