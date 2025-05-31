namespace NewsPaper.src.Application.DTOs
{
    public class AddWatchlistDto
    {
        public string CoinId { get; set; }
        public string CoinSymbol { get; set; }
        public string CoinName { get; set; }
        public string CoinImage { get; set; }
        public int Order { get; set; } = 0;
    }
}