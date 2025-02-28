namespace NewsPaper.src.Application.DTOs
{
    public class ListNewsDtoResponse
    {
        public int NewsID { get; set; }
        public string Header { get; set; }

        public string Title { get; set; }

        public string Links { get; set; }

        public string TimeReading { get; set; }

        public string UserName { get; set; }

        public string? UserAvartar { get; set; }

        public string TimeAgo { get; set; }
    }
}
