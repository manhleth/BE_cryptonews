// src/Application/Services/TransactionService.cs
using AutoMapper;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Domain.Entities;
using NewsPaper.src.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPaper.src.Application.Services
{
    public class TransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Lưu transaction mới vào cơ sở dữ liệu
        public async Task<object> SaveTransaction(TransactionDto transactionDto)
        {
            try
            {
                var transaction = _mapper.Map<Transaction>(transactionDto);
                await _unitOfWork.Transaction.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();
                return transaction;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Lấy lịch sử giao dịch của user
        public async Task<object> GetUserTransactions(int userId)
        {
            try
            {
                var transactions = await _unitOfWork.Transaction.FindAsync(x => x.UserId == userId);
                var transactionDtos = _mapper.Map<List<TransactionResponseDto>>(transactions);

                // Thêm TimeAgo
                foreach (var tx in transactionDtos)
                {
                    tx.TimeAgo = CalculateTimeAgo(tx.CreatedDate);
                }

                return transactionDtos.OrderByDescending(t => t.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Cập nhật trạng thái transaction
        public async Task<object> UpdateTransactionStatus(string transactionHash, string status, decimal? gasUsed = null)
        {
            try
            {
                var transaction = await _unitOfWork.Transaction.FindOnlyByCondition(x => x.TransactionHash == transactionHash);
                if (transaction == null)
                    return "Không tìm thấy giao dịch";

                transaction.Status = status;
                if (gasUsed.HasValue)
                    transaction.GasUsed = gasUsed.Value;

                transaction.ModifiedDate = DateTime.Now;
                await _unitOfWork.Transaction.UpdateAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<TransactionDto>(transaction);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Lấy thông tin chi tiết của một transaction
        public async Task<object> GetTransactionDetail(string transactionHash)
        {
            try
            {
                var transaction = await _unitOfWork.Transaction.FindOnlyByCondition(x => x.TransactionHash == transactionHash);
                if (transaction == null)
                    return "Không tìm thấy giao dịch";

                return _mapper.Map<TransactionDto>(transaction);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Lấy thống kê giao dịch
        public async Task<object> GetTransactionStatistics(int userId)
        {
            try
            {
                var transactions = await _unitOfWork.Transaction.FindAsync(x => x.UserId == userId);

                var stats = new
                {
                    TotalTransactions = transactions.Count(),
                    SuccessfulTransactions = transactions.Count(x => x.Status == "SUCCESS"),
                    FailedTransactions = transactions.Count(x => x.Status == "FAILED"),
                    PendingTransactions = transactions.Count(x => x.Status == "PENDING"),
                    TotalSentETH = transactions
                        .Where(x => x.TransactionType == "SEND" && x.Status == "SUCCESS")
                        .Sum(x => x.FromAmount),
                    TotalReceivedETH = transactions
                        .Where(x => x.TransactionType == "RECEIVE" && x.Status == "SUCCESS")
                        .Sum(x => x.ToAmount),
                    SwapCount = transactions.Count(x => x.TransactionType == "SWAP"),
                    LastTransactionDate = transactions.Any()
                        ? transactions.Max(x => x.CreatedDate)
                        : null
                };

                return stats;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Helper method để tính thời gian trước
        private string CalculateTimeAgo(DateTime? createdDate)
        {
            if (!createdDate.HasValue) return "Không xác định";

            var timeSpan = DateTime.Now - createdDate.Value;

            if (timeSpan.TotalMinutes < 1)
                return "Vừa xong";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            else if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} giờ trước";
            else if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} ngày trước";
            else if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";
            else
                return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
        }
    }
}