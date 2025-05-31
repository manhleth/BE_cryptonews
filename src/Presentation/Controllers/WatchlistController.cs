using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Features;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistController : BaseController<WatchlistController>
    {
        private readonly WatchlistService _watchlistService;
        private readonly ILogger<WatchlistController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WatchlistController(WatchlistService watchlistService, ILogger<WatchlistController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _watchlistService = watchlistService;
        }

        /// <summary>
        /// Lấy danh sách watchlist của user hiện tại
        /// </summary>
        /// <returns>Danh sách watchlist</returns>
        [HttpGet("GetUserWatchlist")]
        public async Task<ResponseData> GetUserWatchlist(int userId)
        {
            try
            {
                // Sử dụng UserIDLogined thay vì parameter userId để tránh lỗi authorization
                var result = await _watchlistService.GetUserWatchlist(UserIDLogined);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user watchlist");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Thêm coin vào watchlist
        /// </summary>
        /// <param name="userId">ID của user (sẽ được override bằng UserIDLogined)</param>
        /// <param name="addWatchlistDto">Thông tin coin cần thêm</param>
        /// <returns>Kết quả thêm</returns>
        [HttpPost("AddToWatchlist")]
        public async Task<ResponseData> AddToWatchlist(int userId, [FromBody] AddWatchlistDto addWatchlistDto)
        {
            try
            {
                // Luôn sử dụng UserIDLogined để đảm bảo security
                var result = await _watchlistService.AddToWatchlist(UserIDLogined, addWatchlistDto);

                // Kiểm tra kết quả trả về
                if (result is string && result.ToString().Contains("already exists"))
                {
                    return new ResponseData { Data = result, StatusCode = 0 }; // Coin đã tồn tại
                }

                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to watchlist: {Message}", ex.Message);
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Xóa coin khỏi watchlist
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <param name="coinId">ID của coin cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("RemoveFromWatchlist")]
        public async Task<ResponseData> RemoveFromWatchlist(int userId, string coinId)
        {
            try
            {
                var result = await _watchlistService.RemoveFromWatchlist(UserIDLogined, coinId);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from watchlist");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Toggle coin trong watchlist (thêm nếu chưa có, xóa nếu đã có)
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <param name="addWatchlistDto">Thông tin coin</param>
        /// <returns>Kết quả toggle</returns>
        [HttpPost("ToggleWatchlist")]
        public async Task<ResponseData> ToggleWatchlist(int userId, [FromBody] AddWatchlistDto addWatchlistDto)
        {
            try
            {
                var result = await _watchlistService.ToggleWatchlist(UserIDLogined, addWatchlistDto);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling watchlist");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Kiểm tra coin có trong watchlist không
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <param name="coinId">ID của coin</param>
        /// <returns>True nếu có trong watchlist</returns>
        [HttpGet("IsInWatchlist")]
        public async Task<ResponseData> IsInWatchlist(int userId, string coinId)
        {
            try
            {
                var result = await _watchlistService.IsInWatchlist(UserIDLogined, coinId);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking watchlist");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// Lấy danh sách coinId trong watchlist của user (cho API khác sử dụng)
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <returns>Danh sách coinId</returns>
        [HttpGet("GetWatchlistCoinIds")]
        public async Task<ActionResult<List<string>>> GetWatchlistCoinIds(int userId)
        {
            try
            {
                var result = await _watchlistService.GetUserWatchlistCoinIds(UserIDLogined);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlist coin IDs");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Admin: Lấy tất cả watchlist (chỉ dành cho admin)
        /// </summary>
        /// <returns>Tất cả watchlist</returns>
        [HttpGet("GetAllWatchlists")]
        [Authorize(Roles = "1")]
        public async Task<ResponseData> GetAllWatchlists()
        {
            try
            {
                // Logic để lấy tất cả watchlist (implement trong service nếu cần)
                return new ResponseData { Data = "Feature not implemented yet", StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all watchlists");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }
    }
}