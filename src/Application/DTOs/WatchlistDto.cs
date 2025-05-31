namespace NewsPaper.src.Application.DTOs
{
    public class WatchlistDto
    {
        public int WatchlistId { get; set; }
        public int UserId { get; set; }
        public string CoinId { get; set; }
        public string CoinSymbol { get; set; }
        public string CoinName { get; set; }
        public string CoinImage { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}