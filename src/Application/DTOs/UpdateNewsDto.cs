namespace NewsPaper.src.Application.DTOs
{
    public class UpdateNewsDto
    {
        public int NewsId { get; set; }
        public string Header { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Footer { get; set; }
        public int? TimeReading { get; set; }
        public string? Links { get; set; }
        public int? CategoryId { get; set; }
        public int? ChildrenCategoryId { get; set; }
        public string? ImagesLink { get; set; }
    }
}