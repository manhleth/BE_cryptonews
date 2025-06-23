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
    public class CategoryController : BaseController<CategoryController>
    {
        private readonly CategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryController(CategoryService categoryService, ILogger<CategoryController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _categoryService = categoryService;
        }

        [HttpGet("GetAllCategories")]
        [AllowAnonymous]
        public async Task<ResponseData> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategory();

                if (categories is string errorMessage)
                {
                    return new ResponseData { Data = errorMessage, StatusCode = -1 };
                }

                return new ResponseData { Data = categories, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all categories");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        [Authorize(Roles = "1")]
        [HttpPost("CreateCategory")]
        public async Task<ResponseData> CreateCategory(CategoryDto categoryDto)
        {
            try
            {
                var newCategory = await _categoryService.CreateCategory(categoryDto);

                // Check if result is error message
                if (newCategory is string errorMessage && (errorMessage.Contains("Lỗi") || errorMessage.Contains("không") || errorMessage.Contains("đã tồn tại")))
                {
                    return new ResponseData { Data = errorMessage, StatusCode = -1 };
                }

                return new ResponseData { Data = newCategory, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        [HttpGet("GetCategoryTop5")]
        [AllowAnonymous]
        public async Task<ResponseData> GetCategoryTop4()
        {
            try
            {
                var categories = await _categoryService.GetTopCategory();

                if (categories is string errorMessage)
                {
                    return new ResponseData { Data = errorMessage, StatusCode = -1 };
                }

                return new ResponseData { Data = categories, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top categories");
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        [Authorize(Roles = "1")]
        [HttpPost("AdminDelete")]
        public async Task<ResponseData> AdminDelete(int id)
        {
            try
            {
                var result = await _categoryService.DelteCateGory(id);

                if (result is string message && (message.Contains("Lỗi") || message.Contains("Không thể") || message.Contains("Không tìm thấy")))
                {
                    return new ResponseData { Data = result, StatusCode = -1 };
                }

                return new ResponseData { Data = result, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID: {Id}", id);
                return new ResponseData { Data = ex.Message, StatusCode = -1 };
            }
        }

        // DEPRECATED - Keep for backward compatibility
        [Authorize(Roles = "1")]
        [HttpDelete("DeleteCategory")]
        public async Task<ResponseData> DeleteCategory(int categoryID)
        {
            _logger.LogWarning("DeleteCategory endpoint is deprecated. Use AdminDelete instead.");
            return await AdminDelete(categoryID);
        }
    }
}