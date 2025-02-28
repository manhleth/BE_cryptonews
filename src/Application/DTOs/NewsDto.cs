namespace NewsPaper.src.Application.DTOs
{
    public class NewsDto
    {
        public string Header { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Footer { get; set; }

        public int? TimeReading { get; set; }

        public string UserName { get; set; }

        public string avatar { get; set; }
        public int? CategoryId { get; set; }
    }
}
