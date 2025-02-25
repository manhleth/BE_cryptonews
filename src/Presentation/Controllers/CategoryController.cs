using AutoMapper;
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
        [HttpGet("GetCategories")]
        public IActionResult GetCategories()
        {
            var categories = _categoryService.SearchNewsAsync();
            return Ok();
        }
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
        [HttpPut("UpdateCategory")]
        public IActionResult UpdateCategory()
        {
            return Ok();
        }
        [HttpDelete("DeleteCategory")]
        public IActionResult DeleteCategory()
        {
            return Ok();
        }

    }
}
