namespace NewsPaper.src.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public string TransactionHash { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string FromToken { get; set; } = "ETH"; // Mặc định là ETH
        public string ToToken { get; set; } = "ETH"; // Mặc định là ETH
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public string TransactionType { get; set; } = "SEND"; // SEND, RECEIVE, SWAP
        public string Status { get; set; } = "PENDING"; // PENDING, SUCCESS, FAILED
        public decimal GasUsed { get; set; }
        public decimal GasPrice { get; set; }
    }
}
