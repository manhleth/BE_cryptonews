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
            var categories = await _categoryService.GetAllCategory();
            return new ResponseData { Data = categories, StatusCode = 1 };
        }
        [Authorize(Roles ="1")]
        [HttpPost("CreateCategory")]
        public async Task<ResponseData> CreateCategory(CategoryDto categoryDto)
        {
            try
            {
                var newCategory = await _categoryService.CreateCategory(categoryDto);
                return new ResponseData { Data = newCategory, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                return new ResponseData { Data = ex, StatusCode = -1 };
            }
        }

        [HttpGet("GetCategoryTop5")]
        [AllowAnonymous]
        public async Task<ResponseData> GetCategoryTop4()
        {
            var categories = await _categoryService.GetTopCategory();
            return new ResponseData { Data = categories, StatusCode = 1 };
        }
        
        [Authorize(Roles = "1")]
        [HttpDelete("DeleteCategory")]
        public async Task<ResponseData> DeleteCategory(int categoryID)
        {
            var category = await _categoryService.DelteCateGory(categoryID);
            return new ResponseData { Data = category, StatusCode = 1 };
        }

    }
}
