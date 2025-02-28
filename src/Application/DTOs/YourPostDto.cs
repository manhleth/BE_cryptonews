namespace NewsPaper.src.Application.DTOs
{
    public class YourPostDto
    {
        public int NewsID { get; set; }
        public string Header { get; set; }

        public string Title { get; set; }

        public string TimeReading { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ImagesLink { get; set; }
    }
}
