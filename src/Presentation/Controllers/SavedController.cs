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
    public class SavedController : BaseController<SavedController>
    {
        private readonly SavedService _savedService;
        private readonly ILogger<SavedController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SavedController(SavedService savedService, ILogger<SavedController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _savedService = savedService;
        }

        /// <summary>
        /// Lấy danh sách ID các bài viết đã lưu (raw data)
        /// </summary>
        /// <returns>Danh sách saved records</returns>
        [HttpGet("GetYourListSaved")]
        public async Task<object> GetListSaved()
        {
            var saved = await _savedService.GetListSavedByUser(UserIDLogined);
            return new ResponseData { Data = saved, StatusCode = 1 };
        }

        /// <summary>
        /// Thêm hoặc bỏ lưu bài viết
        /// </summary>
        /// <param name="newsID">ID của bài viết</param>
        /// <returns>Kết quả thao tác</returns>
        [HttpPost("AddOrRemoveSaved")]
        public async Task<object> CreateSaved(int newsID)
        {
            var newSaved = await _savedService.AddOrRemoveSaved(newsID, UserIDLogined);
            return new ResponseData { Data = newSaved, StatusCode = 1 };
        }

        /// <summary>
        /// [DEPRECATED] Lấy bài viết đã lưu theo category cụ thể - Sử dụng GetAllSavedPosts thay thế
        /// </summary>
        /// <param name="categoryID">ID của category</param>
        /// <returns>Danh sách bài viết đã lưu trong category</returns>
        [HttpGet("GetListSavedPostByUser")]
        [Obsolete("Use GetAllSavedPosts or GetSavedPostsByCategory instead")]
        public async Task<object> GetListSavedPostByUser(int categoryID)
        {
            var saved = await _savedService.GetListSavedPostByUser(UserIDLogined, categoryID);
            return new ResponseData { Data = saved, StatusCode = 1 };
        }

        /// <summary>
        /// [NEW] Lấy TẤT CẢ bài viết đã lưu (category != 0) - Thay thế cho GetListSavedPostByUser
        /// </summary>
        /// <returns>Tất cả bài viết đã lưu với thông tin chi tiết</returns>
        [HttpGet("GetAllSavedPosts")]
        public async Task<object> GetAllSavedPosts()
        {
            try
            {
                var result = await _savedService.GetAllSavedPostsByUser(UserIDLogined);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all saved posts for user {UserId}", UserIDLogined);
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// [NEW] Lấy bài viết đã lưu với filter category (optional)
        /// </summary>
        /// <param name="categoryId">ID của category (optional, null = tất cả categories != 0)</param>
        /// <returns>Bài viết đã lưu theo filter</returns>
        [HttpGet("GetSavedPostsByCategory")]
        public async Task<object> GetSavedPostsByCategory(int? categoryId = null)
        {
            try
            {
                var result = await _savedService.GetSavedPostsByCategory(UserIDLogined, categoryId);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting saved posts by category for user {UserId}", UserIDLogined);
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// [NEW] Lấy thống kê bài viết đã lưu theo category
        /// </summary>
        /// <returns>Thống kê số lượng bài viết đã lưu theo từng category</returns>
        [HttpGet("GetSavedPostsStatistics")]
        public async Task<object> GetSavedPostsStatistics()
        {
            try
            {
                var result = await _savedService.GetSavedPostsStatistics(UserIDLogined);
                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting saved posts statistics for user {UserId}", UserIDLogined);
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// [UTILITY] Kiểm tra bài viết có được lưu hay không
        /// </summary>
        /// <param name="newsId">ID của bài viết</param>
        /// <returns>True nếu bài viết đã được lưu</returns>
        [HttpGet("IsPostSaved")]
        public async Task<object> IsPostSaved(int newsId)
        {
            try
            {
                var saved = await _unitOfWork.Saved.FindOnlyByCondition(x =>
                    x.UserId == UserIDLogined &&
                    x.NewsId == newsId &&
                    x.Status == 1
                );

                return new ResponseData
                {
                    Data = new { isPostSaved = saved != null, newsId = newsId },
                    StatusCode = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if post {NewsId} is saved for user {UserId}", newsId, UserIDLogined);
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        /// <summary>
        /// [UTILITY] Lấy số lượng tổng bài viết đã lưu
        /// </summary>
        /// <returns>Tổng số bài viết đã lưu</returns>
        [HttpGet("GetSavedPostsCount")]
        public async Task<object> GetSavedPostsCount()
        {
            try
            {
                var savedList = await _unitOfWork.Saved.FindAsync(x =>
                    x.UserId == UserIDLogined &&
                    x.Status == 1
                );

                var savedNewsIds = savedList.Select(x => x.NewsId).ToList();
                var validNews = await _unitOfWork.News.FindAsync(x =>
                    savedNewsIds.Contains(x.NewsId) &&
                    x.CategoryId != null &&
                    x.CategoryId != 0
                );

                return new ResponseData
                {
                    Data = new
                    {
                        totalSavedPosts = validNews.Count(),
                        lastUpdated = DateTime.Now
                    },
                    StatusCode = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting saved posts count for user {UserId}", UserIDLogined);
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }
    }
}