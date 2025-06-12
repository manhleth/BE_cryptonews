// src/Presentation/Controllers/TransactionController.cs
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Features;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : BaseController<TransactionController>
    {
        private readonly TransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionController(
            TransactionService transactionService,
            ILogger<TransactionController> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _transactionService = transactionService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Lưu transaction mới
        [HttpPost("SaveTransaction")]
        public async Task<ResponseData> SaveTransaction(TransactionDto transactionDto)
        {
            try
            {
                // Đặt UserId từ token
                transactionDto.UserId = UserIDLogined;

                var result = await _transactionService.SaveTransaction(transactionDto);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu transaction");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        // Lấy lịch sử giao dịch của user
        [HttpGet("GetUserTransactions")]
        public async Task<ResponseData> GetUserTransactions()
        {
            try
            {
                var transactions = await _transactionService.GetUserTransactions(UserIDLogined);
                return new ResponseData { Data = transactions, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách transaction");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        // Lấy chi tiết giao dịch
        [HttpGet("GetTransactionDetail")]
        public async Task<ResponseData> GetTransactionDetail(string transactionHash)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionDetail(transactionHash);
                return new ResponseData { Data = transaction, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết transaction");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        // Cập nhật trạng thái giao dịch
        [HttpPut("UpdateTransactionStatus")]
        public async Task<ResponseData> UpdateTransactionStatus(string transactionHash, string status, decimal? gasUsed = null)
        {
            try
            {
                var result = await _transactionService.UpdateTransactionStatus(transactionHash, status, gasUsed);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái transaction");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        // Lấy thống kê giao dịch của user
        [HttpGet("GetTransactionStatistics")]
        public async Task<ResponseData> GetTransactionStatistics()
        {
            try
            {
                var stats = await _transactionService.GetTransactionStatistics(UserIDLogined);
                return new ResponseData { Data = stats, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê transaction");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }
    }
}