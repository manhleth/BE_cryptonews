namespace NewsPaper.src.Application.DTOs
{
    public class CreateNewsDto
    {
        public string Header { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Footer { get; set; }

        public string? Links { get; set; }

        public int? TimeReading { get; set; }

        public int UserId { get; set; }

        public int? CategoryId { get; set; }
        public int? childrenCategoryId { get; set; }

        public string? ImagesLink { get; set; }
    }
}
