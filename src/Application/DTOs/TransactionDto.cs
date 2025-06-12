using System;

namespace NewsPaper.src.Application.DTOs
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public string TransactionHash { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string FromToken { get; set; } = "ETH";
        public string ToToken { get; set; } = "ETH";
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public decimal GasUsed { get; set; }
        public decimal GasPrice { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class SwapTransactionDto
    {
        public string FromToken { get; set; } = "ETH";
        public string ToToken { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public string FromAddress { get; set; }
        public string SlippageTolerance { get; set; } = "0.5";
    }

    public class TransactionResponseDto
    {
        public int TransactionId { get; set; }
        public string TransactionHash { get; set; }
        public string FromToken { get; set; }
        public string ToToken { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string TimeAgo { get; set; }
    }
}