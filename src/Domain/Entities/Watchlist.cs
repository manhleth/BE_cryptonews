namespace NewsPaper.src.Domain.Entities
{
    public class Watchlist
    {
        public int WatchlistId { get; set; }
        public int UserId { get; set; }
        public string CoinId { get; set; } 
        public string CoinSymbol { get; set; } 
        public string CoinName { get; set; }
        public string CoinImage { get; set; } 
        public int Order { get; set; } = 0;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true; 
    }
}
